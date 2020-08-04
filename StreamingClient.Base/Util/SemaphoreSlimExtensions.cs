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
        /// Awaits on a semaphore, awaits the specified task, &amp; then releases the semaphore.
        /// </summary>
        /// <param name="semaphore">The semaphore to await on</param>
        /// <param name="task">The task to await</param>
        /// <returns>An awaitable task</returns>
        public static async Task WaitAndRelease(this SemaphoreSlim semaphore, Task task)
        {
            try
            {
                await semaphore.WaitAsync();

                await task;
            }
            catch (Exception ex) { Logger.Log(ex); }
            finally { semaphore.Release(); }
        }

        /// <summary>
        /// Awaits on a semaphore, awaits the specified task, &amp; then releases the semaphore.
        /// </summary>
        /// <param name="semaphore">The semaphore to await on</param>
        /// <param name="task">The task to await</param>
        /// <returns>An awaitable task of type T</returns>
        public static async Task<T> WaitAndRelease<T>(this SemaphoreSlim semaphore, Task<T> task)
        {
            try
            {
                await semaphore.WaitAsync();

                return await task;
            }
            catch (Exception ex) { Logger.Log(ex); }
            finally { semaphore.Release(); }
            return default(T);
        }

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
