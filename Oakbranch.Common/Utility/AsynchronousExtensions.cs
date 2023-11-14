using System;
using System.Threading;
using System.Threading.Tasks;

namespace Oakbranch.Common.Utility
{
    /// <summary>
    /// A helper class that provides methods for working with asynchronous operations.
    /// </summary>
    public static class AsynchronousExtensions
    {
        /// <summary>
        /// Waits for the specified wait handle to be set, asynchronously.
        /// <para>Note: this method DOES NOT work on <see cref="Mutex"/></para>
        /// </summary>
        /// <param name="handle">The handle to wait for.</param>
        /// <param name="millisecondsTimeout">
        /// The amount of time to wait for the handle to be set (in milliseconds).
        /// <para>Use the value <c>0</c> to immediately return the handle's state, and <c>-1</c> for no timeout.</para>
        /// </param>
        /// <returns>A task representing the asynchronous operation. The task's result is <see langword="true"/> if the handle was set; otherwise, <see langword="false"/>.</returns>
        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout)
        {
#if DEBUG
            if (handle is Mutex)
            {
                throw new NotSupportedException("This method cannot be executed on instances of the Mutex class.");
            }
#endif
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

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
                registeredHandle?.Unregister(null);
            }
        }

        /// <summary>
        /// Waits for the specified wait handle to be set, asynchronously.
        /// <para>Note: this method DOES NOT work on <see cref="Mutex"/></para>
        /// <para>Throws <see cref="TaskCanceledException"/> if cancellation is requested via <paramref name="cancellationToken"/>.</para>
        /// </summary>
        /// <param name="handle">The handle to wait for.</param>
        /// <param name="millisecondsTimeout">
        /// The amount of time to wait for the handle to be set (in milliseconds).
        /// <para>Use the value <c>0</c> to immediately return the handle's state, and <c>-1</c> for no timeout.</para>
        /// </param>
        /// <param name="cancellationToken">The cancellation token for the waiting operation.</param>
        /// <returns>A task representing the asynchronous operation. The task's result is <see langword="true"/> if the handle was set; otherwise, <see langword="false"/>.</returns>
        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
#if DEBUG
            if (handle is Mutex)
            {
                throw new NotSupportedException("This method cannot be executed on instances of the Mutex class.");
            }
#endif
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

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
                registeredHandle?.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        /// <summary>
        /// Waits for the specified wait handle to be set, asynchronously.
        /// <para>Note: this method DOES NOT work on <see cref="Mutex"/></para>
        /// <para>Throws <see cref="TaskCanceledException"/> if cancellation is requested via <paramref name="cancellationToken"/>.</para>
        /// </summary>
        /// <param name="handle">The handle to wait for.</param>
        /// <param name="cancellationToken">The cancellation token for the waiting operation.</param>
        /// <returns>A task representing the asynchronous operation. The task's result is <see langword="true"/> if the handle was set; otherwise, <see langword="false"/>.</returns>
        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken)
            => handle.WaitOneAsync(Timeout.Infinite, cancellationToken);

        /// <summary>
        /// Represents the given cancellation token as a task that awaits cancellation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to wrap into a task.</param>
        /// <returns>A instance of <see cref="Task"/> that completes upon cancellation.</returns>
        public static Task AsTask(this CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register((o) => ((TaskCompletionSource<bool>)o).TrySetResult(true), tcs);
            return tcs.Task;
        }
    }
}