using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;
using Bytewizer.TinyCLR.DigitalPortal.Client;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public static class NetworkProvider
    {
        private static readonly object _lock = new object();

        private static bool _initialized;

        public static NetworkController Controller { get; private set; }
        public static bool IsLinkReady { get; private set; }
        public static bool IsConnected { get; private set; }

        public static void Initialize()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                var enablePin = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA8);
                enablePin.SetDriveMode(GpioPinDriveMode.Output);
                enablePin.Write(GpioPinValue.High);

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

                var settings = SettingsProvider.Flash;
                Controller.SetInterfaceSettings(new WiFiNetworkInterfaceSettings()
                {
                    Ssid = settings.Ssid,
                    Password = settings.Password,
                });

                Controller.SetAsDefaultController();
                Controller.NetworkLinkConnectedChanged += NetworkController_NetworkLinkConnectedChanged;
                Controller.NetworkAddressChanged += NetworkController_NetworkAddressChanged;

                _initialized = true;
            }

        }

        private static void NetworkController_NetworkLinkConnectedChanged(NetworkController sender, NetworkLinkConnectedChangedEventArgs e)
        {
            IsLinkReady = e.Connected;
        }

        private static void NetworkController_NetworkAddressChanged(NetworkController sender, NetworkAddressChangedEventArgs e)
        {
            var ipProperties = sender.GetIPProperties();
            var address = ipProperties.Address.GetAddressBytes();
            IsConnected = address[0] != 0;

            if (IsConnected)
            {
                ListNetworkInfo(sender);
                ConnectNetworkTime();
                ConnectOpenWeather();
            }
        }

        public static void EnableWifi()
        {
            try
            {
                Controller.SetInterfaceSettings(new WiFiNetworkInterfaceSettings()
                {
                    Ssid = SettingsProvider.Flash.Ssid,
                    Password = SettingsProvider.Flash.Password,
                });
                Controller.Enable();
            }
            catch
            {

            }
        }

        public static void DisableWifi()
        {
            if (IsConnected)
            {
                try
                {
                    Controller.Disable();
                }
                catch
                {

                }
            }
        }

        public static void ConnectGeoLocation()
        {
            var response = GeoClient.Connect();
            if (response.status == "success")
            {
                SettingsProvider.Flash.Lat = response.lat.ToString();
                SettingsProvider.Flash.Lon = response.lon.ToString();
                SettingsProvider.Flash.Query = response.query;
                SettingsProvider.Flash.TimeOffset = response.offset / 60 / 60;
                SettingsProvider.Flash.Location = $"{response.city}, {response.region}";
                SettingsProvider.WriteFlash(SettingsProvider.Flash);
            }
        }

        public static void ConnectNetworkTime()
        {
            if (IsConnected == false)
            {
                return;
            }

            var timeServer = SettingsProvider.Flash.TimeServer;
            var timeOffset = SettingsProvider.Flash.TimeOffset;

            Debug.WriteLine(string.Empty);
            Debug.WriteLine("Synchronize local time with NTP:");
            Debug.WriteLine(string.Empty);
            Debug.WriteLine("   NTP Address . . . . . . . . . . . : " + timeServer);
            Debug.WriteLine("   Time Zone Offset. . . . . . . . . : " + timeOffset);
            Debug.WriteLine(string.Empty);

            try
            {
                DateTime currentTime = GetNtpTime(timeServer, timeOffset);

                if (currentTime != DateTime.MinValue)
                {
                    if (ClockProvider.Controller.IsValid)
                    {
                        ClockProvider.Controller.Now = currentTime;
                    }
                    SystemTime.SetTime(currentTime, timeOffset);
                }

                Debug.WriteLine("Synchronization successfull:");
                Debug.WriteLine(string.Empty);
                Debug.WriteLine("   Local Time. . . . . . . . . . . . : " + DateTime.Now.ToUniversalTime().ToString("R"));
                Debug.WriteLine(string.Empty);

            }
            catch
            {
                Debug.WriteLine("Synchronization failed:");
                Debug.WriteLine(string.Empty);
                Debug.WriteLine("   Local Time. . . . . . . . . . . . : " + DateTime.Now.ToUniversalTime().ToString("R"));
                Debug.WriteLine(string.Empty);
            }
        }

        public static void ConnectOpenWeather()
        {
            var response = WeatherProvider.Connect();
            if (response != null)
            {
                Debug.WriteLine("Retreive weather data from OpenWeather:");
                Debug.WriteLine("   Temperature . . . . . . . . . . . : " + response?.Temp);
            }
        }

        private static DateTime GetNtpTime(string timeServer, int timeZoneOffset)
        {
            var address = Dns.GetHostEntry(timeServer).AddressList[0];

            if (address == null)
                return DateTime.MinValue;

            IPEndPoint ep = new IPEndPoint(Dns.GetHostEntry(timeServer).AddressList[0], 123);

            byte[] ntpData = new byte[48];

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.SendTimeout = socket.ReceiveTimeout = 10000;
                socket.Connect(ep);
                ntpData[0] = 0x1B;
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            const byte offsetTransmitTime = 40;

            ulong intpart = 0;
            ulong fractpart = 0;

            for (int i = 0; i <= 3; i++)
            {
                intpart = (intpart << 8) | ntpData[offsetTransmitTime + i];
            }

            for (int i = 4; i <= 7; i++)
            {
                fractpart = (fractpart << 8) | ntpData[offsetTransmitTime + i];
            }

            ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

            TimeSpan timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
            DateTime dateTime = new DateTime(1900, 1, 1);
            dateTime += timeSpan;

            TimeSpan offsetAmount = new TimeSpan(timeZoneOffset, 0, 0);
            DateTime networkDateTime = (dateTime + offsetAmount);

            return networkDateTime;
        }

        private static void ListNetworkInfo(NetworkController networkInterface)
        {
            try
            {
                switch (networkInterface.InterfaceType)
                {
                    case (NetworkInterfaceType.Ethernet):
                        Debug.WriteLine(string.Empty);
                        Debug.WriteLine("Found Ethernet Interface");
                        break;
                    case (NetworkInterfaceType.WiFi):
                        Debug.WriteLine(string.Empty);
                        Debug.WriteLine("Found 802.11 WiFi Interface");
                        break;
                    case (NetworkInterfaceType.Ppp):
                        Debug.WriteLine(string.Empty);
                        Debug.WriteLine("Found Point to Point Interface");
                        break;
                }

                var network = networkInterface.GetInterfaceProperties();
                var ip = networkInterface.GetIPProperties();
                var activeSettings = networkInterface.ActiveInterfaceSettings;

                Debug.WriteLine(string.Empty);
                Debug.WriteLine("   Physical Address. . . . . . . . . : " + GetPhysicalAddress(network.MacAddress));
                Debug.WriteLine("   DHCP Enabled. . . . . . . . . . . : " + BoolToYesNo(activeSettings.DhcpEnable));
                Debug.WriteLine("   IPv4 Address. . . . . . . . . . . : " + ip.Address.ToString());
                Debug.WriteLine("   Subnet Mask . . . . . . . . . . . : " + ip.SubnetMask.ToString());
                Debug.WriteLine("   Default Gateway . . . . . . . . . : " + ip.GatewayAddress.ToString());
                Debug.WriteLine(string.Empty);
                for (int i = 0; i < ip.DnsAddresses.Length; i++)
                {
                    Debug.WriteLine("   DNS Servers . . . . . . . . . . . : " + ip.DnsAddresses[i].ToString());
                }
                Debug.WriteLine("   Dynamic DNS Enabled . . . . . . . : " + BoolToYesNo(activeSettings.DynamicDnsEnable));
                Debug.WriteLine(string.Empty);

                if (networkInterface.InterfaceType == NetworkInterfaceType.WiFi)
                {
                    var wifi = (WiFiNetworkInterfaceSettings)networkInterface.ActiveInterfaceSettings;
                    Debug.WriteLine("   SSID  . . . . . . . . . . . . . . : " + wifi.Ssid);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ListNetworkInfo exception:  " + e.Message);
            }
        }

        public static string GetPhysicalAddress(byte[] byteArray, char seperator = '-')
        {
            if (byteArray == null)
                return null;

            string physicalAddress = string.Empty;

            for (int i = 0; i < byteArray.Length; i++)
            {
                physicalAddress += byteArray[i].ToString("X2");

                if (i != byteArray.Length - 1)
                    physicalAddress += seperator;
            }

            return physicalAddress;
        }

        public static string BoolToYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }
    }
}