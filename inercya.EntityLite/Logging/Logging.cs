using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET452
namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        void LogError(Exception ex, string message);
        void LogInformation(string format, params object[] parameters);
    }

    public interface ILoggerFactory
    {
        ILogger CreateLogger(string categoryName);
        ILogger CreateLogger<T>();
    }

    public static class LoggerFactoryExtensions 
    {
        public static ILogger CreateLogger<T>(this ILoggerFactory loggerFactory)
        {
            return null;
        }

    }

    namespace Abstractions
    {

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
