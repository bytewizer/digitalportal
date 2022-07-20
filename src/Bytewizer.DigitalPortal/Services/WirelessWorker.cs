using System;

using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Hosting;

using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WirelessWorker : SchedulerService
    {
        private readonly ILogger _logger;
        private readonly WirelessService _network;

        public WirelessWorker(WirelessService network, ILoggerFactory loggerFactory)
            : base(TimeSpan.FromMinutes(1))
        {
            _logger = loggerFactory.CreateLogger(nameof(WirelessWorker));          
            _network = network;
        }

        protected override void ExecuteAsync()
        {
            if (SettingsService.NetworkConnected)
            {
                return;
            }

            _network.Disable();
            _network.Enable();
        }

        public override void Start()
        {
            if (SettingsService.NetworkConnected)
            {
                return;
            }

            var flash = SettingsService.Flash;
            _network.Controller.SetInterfaceSettings(new WiFiNetworkInterfaceSettings()
            {
                Ssid = flash.Ssid,
                Password = flash.Password,
            });

            _network.Enable();
            _logger.HostStarted();

            base.Start();
        }

        public override void Stop()
        {
            _network.Disable();
            _logger.HostStopped();
            
            base.Stop();
        }
    }
}
