using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public class PeekableStreamReader : IAsyncLineReader
    {
        private readonly int _maxLineLength;
        private readonly Queue<string> _peek = new Queue<string>();
        private Stream _stream;
        private byte[] _buf = new byte[4096];
        private int _bufPos = 0;
        private int _bufLength = 0;
        private bool _eof = false;

        public PeekableStreamReader(Stream stream, int maxLineLength = Int32.MaxValue)
        {
            // This is arbitrary, but ~8k lines are not abnormal
            if (maxLineLength < 32768)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLineLength),
                    "The maximum line length must be at least 32768");
            }
            _maxLineLength = maxLineLength;
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        private void ShuffleConsumedBytes(byte[] dst)
        {
            if (_bufPos < _bufLength)
            {
                Buffer.BlockCopy(_buf, _bufPos, dst, 0, _bufLength - _bufPos);
            }
            _bufLength -= _bufPos;
            _bufPos = 0;
        }

        private void CheckDisposed()
        {
            if (_stream == null)
            {
                throw new ObjectDisposedException("PeekableStreamReader");
            }
        }

        private async Task<string> ReadLineNoPeekAsync(CancellationToken cancellationToken)
        {

            for (; ; )
            {
                // read buffered data
                if (_bufPos < _bufLength)
                {
                    int nlPos = _eof ? _bufLength - 1 : Array.IndexOf(_buf, (byte)'\n', _bufPos, _bufLength - _bufPos);
                    if (nlPos >= 0)
                    {
                        string result = Encoding.UTF8.GetString(_buf, _bufPos, nlPos - _bufPos + 1);
                        _bufPos = nlPos + 1;
                        return result;
                    }
                }
                if (_eof)
                {
                    return null;
                }
                // consume any previously read data
                if (_bufPos > 0)
                {
                    ShuffleConsumedBytes(_buf);
                }
                // make the buffer bigger if needed
                int left = _buf.Length - _bufLength;
                if (left < 512)
                {
                    int newlen = Math.Min(_maxLineLength / 2, _buf.Length) * 2;
                    if (newlen <= _buf.Length)
                    {
                        throw new InvalidOperationException(
                            "Reading from the stream failed because a line from the server was unexpectedly " +
                            $"greater than the configured maximum line length of {_maxLineLength}");
                    }
                    byte[] newbuf = new byte[newlen];
                    ShuffleConsumedBytes(newbuf);
                    _buf = newbuf;
                }

                int read = await _stream.ReadAsync(_buf, _bufLength,
                    _buf.Length - _bufLength, cancellationToken).ConfigureAwait(false);
                _bufLength += read;
                _eof = read == 0;
            }
        }

        public async Task<string> ReadLineAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            if (_peek.Count > 0)
            {
                return _peek.Dequeue();
            }
            return await ReadLineNoPeekAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> PeekLineAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            var line = await ReadLineNoPeekAsync(cancellationToken).ConfigureAwait(false);
            _peek.Enqueue(line);
            return line;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }
    }
}
