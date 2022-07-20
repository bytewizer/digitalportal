using System;
using Bytewizer.TinyCLR.Logging;

namespace Bytewizer.TinyCLR.DigitalPortal
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    
    public static class DiagnosticsLoggerExtensions
    {
        private static readonly string _eventName = "Hosted Service";

        public static void HostStarted(this ILogger logger)
        {
            logger.Log(
                LogLevel.Debug,
                new EventId(100, _eventName),
                $"Hosted service started.",
                null
                );
        }

        public static void HostStopped(this ILogger logger)
        {
            logger.Log(
                LogLevel.Information,
                new EventId(110, _eventName),
                $"Hosted service stopped.",
                null
                );
        }

        public static void InterfaceStatus(this ILogger logger, string status)
        {
            logger.Log(
                LogLevel.Information,
                new EventId(115, "802.11 WiFi"),
                "802.11 wireless interface {0}.",
                status
                );
        }
    }
}
