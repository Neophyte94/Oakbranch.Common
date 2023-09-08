using System;
using System.Threading;
using System.Threading.Tasks;

namespace Oakbranch.Common.Utility
{
    public static class AsynchronousExtensions
    {
        /// <summary>
        /// DOES NOT work on <see cref="Mutex"/>
        /// <para>Returns <c>True</c> if the specified handle signals before the timeout interval elapses, otherwise <c>False</c>.</para>
        /// <para>The timeout value <c>0</c> makes the task return the state of the handle immediately.</para>
        /// <para>The timeout value <c>-1</c> makes the timeout interval never elapse.</para>
        /// </summary>
        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout)
        {
            RegisteredWaitHandle registeredHandle = null;
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    tcs,
                    millisecondsTimeout,
                    true);
                return await tcs.Task;
            }
            finally
            {
                if (registeredHandle != null)
                    registeredHandle.Unregister(null);
            }
        }

        /// <summary>
        /// DOES NOT work on <see cref="Mutex"/>
        /// <para>Throws <see cref="TaskCanceledException"/> if cancellation is request via <paramref name="cancellationToken"/>.</para>
        /// </summary>
        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            RegisteredWaitHandle registeredHandle = null;
            CancellationTokenRegistration tokenRegistration = default;
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    tcs,
                    millisecondsTimeout,
                    true);
                tokenRegistration = cancellationToken.Register(
                    state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
                    tcs);
                return await tcs.Task;
            }
            finally
            {
                if (registeredHandle != null)
                    registeredHandle.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        /// <summary>
        /// DOES NOT work on <see cref="Mutex"/>
        /// <para>Throws <see cref="TaskCanceledException"/> if cancellation is request via <paramref name="cancellationToken"/>.</para>
        /// </summary>
        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken) => 
            handle.WaitOneAsync(Timeout.Infinite, cancellationToken);

        public static Task AsTask(this CancellationToken token)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            token.Register((o) => ((TaskCompletionSource<bool>)o).TrySetResult(true), tcs);
            return tcs.Task;
        }
    }
}
