﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if (NET452 || NET35)
namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        void LogError(Exception ex, string message);
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
        public void LogInformation(string format, params object[] args)
        {
            this.logger.Info(format, args);
        }

        public void LogDebug(string format, params object[] args)
        {
            this.logger.Debug(format, args);
        }

        public void LogTrace(string format, params object[] args)
        {
            this.logger.Trace(format, args);
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
