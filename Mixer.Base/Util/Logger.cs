using System;

namespace Mixer.Base.Util
{
    public static class Logger
    {
        public static event EventHandler<string> LogOccurred = delegate { };

        public static void Log(string message)
        {
            try
            {
                Logger.LogOccurred(null, message);
            }
            catch (Exception) { }
        }

        public static void Log(Exception ex)
        {
            Logger.Log(ex.ToString());
        }
    }
}
