using System;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingClient.Base.Util
{
    /// <summary>
    /// Extension methods for the SemaphoreSlim class.
    /// </summary>
    public static class SemaphoreSlimExtensions
    {
        /// <summary>
        /// Awaits on a semaphore, runs the specified function, &amp; then releases the semaphore.
        /// </summary>
        /// <param name="semaphore">The semaphore to await on</param>
        /// <param name="function">The function to run</param>
        /// <returns>An awaitable task</returns>
        public static async Task WaitAndRelease(this SemaphoreSlim semaphore, Func<Task> function)
        {
            try
            {
                await semaphore.WaitAsync();

                await function();
            }
            catch (Exception ex) { Logger.Log(ex); }
            finally { semaphore.Release(); }
        }

        /// <summary>
        /// Awaits on a semaphore, runs the specified function, &amp; then releases the semaphore.
        /// </summary>
        /// <param name="semaphore">The semaphore to await on</param>
        /// <param name="function">The function to run</param>
        /// <returns>An awaitable task of type T</returns>
        public static async Task<T> WaitAndRelease<T>(this SemaphoreSlim semaphore, Func<Task<T>> function)
        {
            try
            {
                await semaphore.WaitAsync();

                return await function();
            }
            catch (Exception ex) { Logger.Log(ex); }
            finally { semaphore.Release(); }
            return default(T);
        }
    }
}
