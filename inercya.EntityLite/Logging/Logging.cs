using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#if (NET452 || NET35)
namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        void LogError(Exception ex, string message);
        void LogError(Exception ex, string message, params object[] parameters);
        void LogWarning(string message);
        void LogWarning(string format, params object[] parameters);
        void LogWarning(Exception ex, string format, params object[] parameters);
        void LogInformation(string format, params object[] parameters);
        void LogDebug(string format, params object[] parameters);
        void LogTrace(string format, params object[] parameters);
    }

    public interface ILoggerFactory
    {
        ILogger CreateLogger(string categoryName);
        ILogger CreateLogger<T>();
    }

    internal class NLogLogger : ILogger
    {
        private readonly NLog.ILogger logger;

        public NLogLogger(NLog.ILogger logger)
        {
            this.logger = logger;
        }

        public void LogError(Exception ex, string message)
        {
            this.logger.Error(ex, message);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            this.logger.Error(ex, message);
        }
        public void LogInformation(string format, params object[] args)
        {
            this.logger.Info(CultureInfo.InvariantCulture, format, args);
        }

        public void LogDebug(string format, params object[] args)
        {
            this.logger.Debug(CultureInfo.InvariantCulture, format, args);
        }

        public void LogTrace(string format, params object[] args)
        {
            this.logger.Trace(CultureInfo.InvariantCulture, format, args);
        }

        public void LogWarning(string message)
        {
            this.logger.Warn(message);
        }

        public void LogWarning(string format, params object[] parameters)
        {
            this.logger.Warn(CultureInfo.InvariantCulture, format, parameters);
        }

        public void LogWarning(Exception ex, string format, params object[] parameters)
        {
            this.logger.Warn(ex, format, parameters);
        }
    }

    internal class NLogFactory: ILoggerFactory
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new NLogLogger(NLog.LogManager.GetLogger(categoryName));
        }
        public ILogger CreateLogger<T>()
        {
            return new NLogLogger(NLog.LogManager.GetLogger(typeof(T).FullName));
        }
    }
}
#endif
