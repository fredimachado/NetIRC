using System;
using System.Threading.Tasks;

namespace NetIRC.Extensions
{
    internal static class TaskExtensions
    {
        // By Brandon Minnick
        // https://github.com/brminnick/AsyncAwaitBestPractices
        public static async void SafeFireAndForget(this Task task,
                                                   bool continueOnCapturedContext = true,
                                                   Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex) when (onException != null)
            {
                onException(ex);
            }
        }
    }
}
