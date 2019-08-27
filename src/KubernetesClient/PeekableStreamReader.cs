using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    internal class PeekableStreamReader : IAsyncLineReader
    {
        private readonly Queue<string> _peek = new Queue<string>();
        private Stream _stream;
        private byte[] _buf = new byte[4096];
        private int _bufPos = 0;
        private int _bufLength = 0;
        private bool _eof = false;

        public PeekableStreamReader(Stream stream)
        {
            _stream = stream;
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

        public async Task<string> ReadLineAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            if (_peek.Count > 0)
            {
                return _peek.Dequeue();
            }

            for (;;)
            {
                // read buffered data
                if (_bufPos < _bufLength)
                {
                    int nlPos = _eof ? _bufLength - 1 : Array.IndexOf(_buf, (byte)'\n', _bufPos, _bufLength -_bufPos);
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
                if (_buf.Length - _bufLength < 512)
                {
                    if (_buf.Length > Int32.MaxValue / 2)
                    {
                        throw new InvalidOperationException(
                            "trying to read a line > 2Gb from the server");
                    }
                    byte[] newbuf = new byte[_buf.Length * 2];
                    ShuffleConsumedBytes(newbuf);
                    _buf = newbuf;
                }

                int read = await _stream.ReadAsync(_buf, _bufLength,
                    _buf.Length - _bufLength, cancellationToken).ConfigureAwait(false);
                _bufLength += read;
                _eof = read == 0;
            }
        }

        public async Task<string> PeekLineAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            var line = await ReadLineAsync(cancellationToken).ConfigureAwait(false);
            _peek.Enqueue(line);
            return line;
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }
    }
}
