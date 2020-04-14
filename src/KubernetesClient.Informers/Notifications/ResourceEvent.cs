using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace k8s.Informers.Notifications
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    [DebuggerStepThrough]
    public struct ResourceEvent<TResource>
    {
        public ResourceEvent(EventTypeFlags eventFlags, TResource value, TResource oldValue = default)
        {
            if (eventFlags.HasFlag(EventTypeFlags.ResetEmpty) || eventFlags.HasFlag(EventTypeFlags.ResetEmpty))
            {
                eventFlags |= EventTypeFlags.ResetStart | EventTypeFlags.ResetEnd;
            }

            if (eventFlags.HasFlag(EventTypeFlags.ResetEnd) || eventFlags.HasFlag(EventTypeFlags.ResetStart))
            {
                eventFlags |= EventTypeFlags.Reset;
            }

            if (eventFlags.HasFlag(EventTypeFlags.Reset) || eventFlags.HasFlag(EventTypeFlags.Sync))
            {
                eventFlags |= EventTypeFlags.Current;
            }

            Value = value;
            OldValue = oldValue;
            EventFlags = eventFlags;
        }

        public EventTypeFlags EventFlags { get; }

        public TResource OldValue { get; }
        public TResource Value { get; }
        public static ResourceEvent<TResource> ResetEmpty { get; } = new ResourceEvent<TResource>(EventTypeFlags.ResetEmpty, default);

        public override string ToString()
        {
            var includePrefix = Value != null && OldValue != null;

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("   ");
            sb.Append(EventFlags);
            sb.Append(": [");
            if (Value != null)
            {
                if (includePrefix)
                {
                    sb.Append(nameof(Value));
                    sb.Append("{ ");
                }

                sb.Append(Value);
                if (includePrefix)
                {
                    sb.Append("} ");
                }
            }

            if (OldValue != null)
            {
                if (includePrefix)
                {
                    sb.Append(nameof(OldValue));
                    sb.Append("{ ");
                }

                sb.Append(OldValue);
                if (includePrefix)
                {
                    sb.Append("} ");
                }
            }

            sb.Append("]");
            return sb.ToString();
        }
    }

    public static class ResourceEventExtensions
    {
        public static ResourceEvent<T> ToResourceEvent<T>(this T obj, EventTypeFlags typeFlags, T oldValue = default)
        {
            if (typeFlags.HasFlag(EventTypeFlags.Delete) && oldValue == null)
            {
                oldValue = obj;
            }
            return new ResourceEvent<T>(typeFlags, obj, oldValue);
        }

        /// <summary>
        ///     Converts a list of objects to a resource reset list event block. Every item is of type <see cref="EventTypeFlags.Reset" />,
        ///     with first and last elements also having <see cref="EventTypeFlags.ResetStart" /> and <see cref="EventTypeFlags.ResetEnd" />
        ///     set respectively. If <paramref name="source" /> is empty and <paramref name="emitEmpty" /> is set,
        /// </summary>
        /// <param name="source">The source enumerable</param>
        /// <param name="emitEmpty">
        ///     If <see langword="true" /> the resulting <see cref="IEnumerable{T}" /> will contain a single
        ///     <see cref="ResourceEvent{TResource}" /> with no object value and <see cref="EventTypeFlags.ResetEmpty" /> flag set
        /// </param>
        /// <typeparam name="TResource">The type of resource</typeparam>
        /// <returns>The resulting enumerable of reset events</returns>
        public static IEnumerable<ResourceEvent<TResource>> ToReset<TResource>(this IEnumerable<TResource> source, bool emitEmpty = false)
        {
            var i = 0;
            using var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                if (emitEmpty)
                {
                    yield return new ResourceEvent<TResource>(EventTypeFlags.ResetEmpty, default);
                }
                yield break;
            }

            var current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                if (i == 0)
                {
                    yield return current.ToResourceEvent(EventTypeFlags.ResetStart);
                }
                else
                {
                    yield return current.ToResourceEvent(EventTypeFlags.Reset);
                }
                current = enumerator.Current;
                i++;
            }

            if (i == 0)
            {
                yield return current.ToResourceEvent(EventTypeFlags.ResetStart | EventTypeFlags.ResetEnd);
            }
            else
            {
                yield return current.ToResourceEvent(EventTypeFlags.ResetEnd);
            }
        }
    }
}
