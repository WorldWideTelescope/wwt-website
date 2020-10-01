using System;
using System.Threading.Channels;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Open.ChannelExtensions;

namespace PlateManager
{
    internal static class ChannelExtensions
    {
        public static ChannelReader<TResult> TransformMany<T, TResult>(this ChannelReader<T> source, Func<T, IEnumerable<TResult>> transform, int capacity, CancellationToken token)
             => Channel.CreateBounded<TResult>(capacity)
                    .Source(TransformChannelToAsyncEnumerable(source, transform, token), token);

        private static async IAsyncEnumerable<TResult> TransformChannelToAsyncEnumerable<T, TResult>(ChannelReader<T> source, Func<T, IEnumerable<TResult>> transform, [EnumeratorCancellation] CancellationToken token)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (transform is null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            await foreach (var item in source.ReadAllAsync(token))
            {
                foreach (var child in transform(item))
                {
                    yield return child;
                }
            }
        }
    }
}
