using System;
using log4net;

namespace SiteBlue
{
    public enum LogLevel
    {
        Warn,
        Error,
        Debug,
        Info,
        Fatal
    }

    public static class Logger
    {
        private static readonly ILog logger = LogManager.GetLogger("WebLogger");

        public static void Log(string msg, Exception ex, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Warn: if (logger.IsWarnEnabled) logger.Warn(msg, ex);
                    break;
                case LogLevel.Error: if (logger.IsErrorEnabled) logger.Error(msg, ex);
                    break;
                case LogLevel.Debug: if (logger.IsDebugEnabled) logger.Debug(msg, ex);
                    break;
                case LogLevel.Info: if (logger.IsInfoEnabled) logger.Info(msg, ex);
                    break;
                case LogLevel.Fatal: if (logger.IsFatalEnabled) logger.Fatal(msg, ex);
                    break;
            }
        }
    }
}