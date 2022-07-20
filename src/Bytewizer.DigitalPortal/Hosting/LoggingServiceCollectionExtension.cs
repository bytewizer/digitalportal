using System;

using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Logging.Debug;
using Bytewizer.TinyCLR.DependencyInjection;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public static class LoggingServiceCollectionExtension
    {
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            return services.AddLogging(LogLevel.Information);
        }

        public static IServiceCollection AddLogging(this IServiceCollection services, LogLevel level)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(level);

            services.TryAdd(new ServiceDescriptor(typeof(ILoggerFactory), loggerFactory));

            return services;
        }
    }
}