using System;
using System.Text;
using System.Threading;

using Bytewizer.TinyCLR.Logging;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WirelessService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly GpioPin _resetPin;
        private readonly SettingsService _settings;
        
        public NetworkController Controller { get; private set; }
        
        public WirelessService(ILoggerFactory loggerFactory, SettingsService settings)
        {
            _logger = loggerFactory.CreateLogger(nameof(WirelessService));
            _settings = settings;

            _resetPin = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA8);
            _resetPin.SetDriveMode(GpioPinDriveMode.Output);

            Reset();

            Controller = NetworkController.FromName(SC20260.NetworkController.ATWinc15x0);

            Controller.SetCommunicationInterfaceSettings(new SpiNetworkCommunicationInterfaceSettings()
            {
                SpiApiName = SC20260.SpiBus.Spi3,
                GpioApiName = SC20260.GpioPin.Id,
                InterruptPin = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PF10),
                InterruptEdge = GpioPinEdge.FallingEdge,
                InterruptDriveMode = GpioPinDriveMode.InputPullUp,
                ResetPin = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PC3),
                ResetActiveState = GpioPinValue.Low,
                SpiSettings = new SpiConnectionSettings()
                {
                    ChipSelectLine = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA6),
                    ClockFrequency = 4000000,
                    Mode = SpiMode.Mode0,
                    ChipSelectType = SpiChipSelectType.Gpio,
                    ChipSelectHoldTime = TimeSpan.FromTicks(10),
                    ChipSelectSetupTime = TimeSpan.FromTicks(10)
                }
            });

            Controller.SetAsDefaultController();
            Controller.NetworkLinkConnectedChanged += NetworkLinkConnectedChanged;
            Controller.NetworkAddressChanged += NetworkAddressChanged;
        }

        public void Dispose()
        {
            _resetPin?.Dispose();
            Controller?.Dispose();
        }

        public void Reset()
        {
            try
            {
                _resetPin.Write(GpioPinValue.Low);
                Thread.Sleep(100);
                _resetPin.Write(GpioPinValue.High);
                
                _logger.InterfaceStatus("reset");

            }
            catch  (Exception ex)
            {
                _logger.LogError(ex, "802.11 wireless interface reset error.");
            }
        }

        public void Enable()
        {
            try 
            {
                Controller.Enable();
            }
            catch 
            {
                _logger.LogError("802.11 wireless interface failed to connect verify ssid and password.");
            }
        }

        public void Disable()
        {
            try
            {
                Controller.Disable();
            }
            catch
            {
                _logger.LogError("Failed to disconnect wireless.");
            }
        }

        private void NetworkLinkConnectedChanged(NetworkController sender, NetworkLinkConnectedChangedEventArgs args)
        {
            if (args.Connected)
            {
                _logger.InterfaceStatus("connected");
            }
            else
            {
                _settings.SetNetworkConnected(false);
                _logger.InterfaceStatus("disconnected");
            }
        }

        private void NetworkAddressChanged(NetworkController sender, NetworkAddressChangedEventArgs args)
        {
            var ipProperties = sender.GetIPProperties();
            var address = ipProperties.Address.GetAddressBytes();

            if (address != null && address[0] != 0 && address.Length > 0)
            {
                var info = GetNetworkInfo(sender);
                _logger.LogInformation(info);
            }

            _settings.SetNetworkConnected(address[0] != 0);
        }

        private string GetNetworkInfo(NetworkController controller)
        {
            var ipProperties = controller.GetIPProperties();
            var physicalAddress = GetPhysicalAddress(controller.GetInterfaceProperties().MacAddress);
            var dhcpEnabled = controller.ActiveInterfaceSettings.DhcpEnable ? "Yes" : "No";

            var sb = new StringBuilder();

            sb.Append($"DHCP Enable: {dhcpEnabled} ");
            sb.Append($"IPv4 Address: {ipProperties.Address} ");
            sb.Append($"Physical Address: {physicalAddress} ");
            sb.Append($"Subnet: {ipProperties.SubnetMask} ");
            sb.Append($"Gateway: {ipProperties.GatewayAddress} ");

            for (int i = 0; i < ipProperties.DnsAddresses.Length; i++)
            {
                var address = ipProperties.DnsAddresses[i].GetAddressBytes();
                if (address[0] != 0)
                {
                    sb.Append($"DNS: {ipProperties.DnsAddresses[i]}");
                    
                    if (i < ipProperties.DnsAddresses.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            
            return sb.ToString();
        }

        private string GetPhysicalAddress(byte[] bytes, char seperator = '-')
        {
            if (bytes == null)
            {
                return null;
            }

            string physicalAddress = string.Empty;

            for (int i = 0; i < bytes.Length; i++)
            {
                physicalAddress += bytes[i].ToString("X2");

                if (i != bytes.Length - 1)
                    physicalAddress += seperator;
            }

            return physicalAddress;
        }
    }
}
