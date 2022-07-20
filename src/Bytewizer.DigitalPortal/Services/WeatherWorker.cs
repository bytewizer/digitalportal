using System;

using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Hosting;
using System.Threading;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WeatherWorker : SchedulerService
    {
        private readonly ILogger _logger;
        private readonly WirelessService _wireless;
        private readonly WeatherService _weather;
       
        public WeatherWorker(WeatherService weather, WirelessService wireless, ILoggerFactory loggerFactory)
            : base(TimeSpan.FromMinutes(10))
        {
            _logger = loggerFactory.CreateLogger(nameof(WeatherWorker));
            _weather = weather;
            _wireless = wireless;
        }

        public override void Start()
        {
            _logger.HostStarted();

            base.Start();
        }

        protected override void ExecuteAsync()
        {
            if (SettingsService.NetworkConnected)
            {
                _weather.Connect();
            }
            else
            {
                while (!SettingsService.NetworkConnected)
                {
                    Thread.Sleep(1000);
                }

                _weather.Connect();
            }
        }

        public override void Stop()
        {
            _logger.HostStopped();

            base.Stop();
        }
    }
}