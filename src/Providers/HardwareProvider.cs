using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Devices.Rtc;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Devices.Network;
using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;

namespace Bytewizer.TinyCLR.DigitalClock
{
    public static class HardwareProvider
    {
        private static readonly object _lock = new object();

        private static bool _initialized;

        public static RtcController Rtc { get; private set; }
        public static DisplayController Display { get; private set; }
        public static FT5xx6Controller TouchController { get; private set; }
        public static NetworkController Network { get; private set; }
        public static bool IsLinkReady { get; private set; }
        public static bool IsNetworkConnected { get; private set; }

        public static int Width => Display.ActiveConfiguration.Width;
        public static int Height => Display.ActiveConfiguration.Height;

        public static void Initialize()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                // RTC
                Rtc = RtcController.GetDefault();

                Rtc.SetChargeMode(BatteryChargeMode.Fast);

                if (Rtc.IsValid)
                {
                    SystemTime.SetTime(Rtc.Now);
                }

                // Display
                var backlight = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA15);

                backlight.SetDriveMode(GpioPinDriveMode.Output);
                backlight.Write(GpioPinValue.High);

                Display = DisplayController.GetDefault();

                Display.SetConfiguration(new ParallelDisplayControllerSettings
                {
                    Width = 480,
                    Height = 272,
                    DataFormat = DisplayDataFormat.Rgb565,
                    Orientation = DisplayOrientation.Degrees0,
                    PixelClockRate = 10000000,
                    PixelPolarity = false,
                    DataEnablePolarity = false,
                    DataEnableIsFixed = false,
                    HorizontalFrontPorch = 2,
                    HorizontalBackPorch = 2,
                    HorizontalSyncPulseWidth = 41,
                    HorizontalSyncPolarity = false,
                    VerticalFrontPorch = 2,
                    VerticalBackPorch = 2,
                    VerticalSyncPulseWidth = 10,
                    VerticalSyncPolarity = false,
                });

                Display.Enable();

                // Touch
                var i2cController = I2cController.FromName(SC20260.I2cBus.I2c1);

                var I2cSettings = new I2cConnectionSettings(0x38)
                {
                    BusSpeed = 100000,
                    AddressFormat = I2cAddressFormat.SevenBit,
                };

                var i2cDevice = i2cController.GetDevice(I2cSettings);

                var gpioController = GpioController.GetDefault();
                var interrupt = gpioController.OpenPin(SC20260.GpioPin.PJ14);

                TouchController = new FT5xx6Controller(i2cDevice, interrupt);

            }

            // Network
            var enablePin = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PI0);
            enablePin.SetDriveMode(GpioPinDriveMode.Output);
            enablePin.Write(GpioPinValue.High);

            Network = NetworkController.FromName(SC20260.NetworkController.ATWinc15x0);

            Network.SetCommunicationInterfaceSettings(new SpiNetworkCommunicationInterfaceSettings()
            {
                SpiApiName = SC20260.SpiBus.Spi3,
                GpioApiName = SC20260.GpioPin.Id,
                InterruptPin = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PG6),
                InterruptEdge = GpioPinEdge.FallingEdge,
                InterruptDriveMode = GpioPinDriveMode.InputPullUp,
                ResetPin = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PI8),
                ResetActiveState = GpioPinValue.Low,
                SpiSettings = new SpiConnectionSettings()
                {
                    ChipSelectLine = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PG12),
                    ClockFrequency = 4000000,
                    Mode = SpiMode.Mode0,
                    ChipSelectType = SpiChipSelectType.Gpio,
                    ChipSelectHoldTime = TimeSpan.FromTicks(10),
                    ChipSelectSetupTime = TimeSpan.FromTicks(10)
                }
            });

            var settings = SettingsProvider.Settings;
            Network.SetInterfaceSettings(new WiFiNetworkInterfaceSettings()
            {
                Ssid = settings.Ssid,
                Password = settings.Password,
            });

            Network.SetAsDefaultController();
            Network.NetworkLinkConnectedChanged += NetworkController_NetworkLinkConnectedChanged;
            Network.NetworkAddressChanged += NetworkController_NetworkAddressChanged;

            _initialized = true;
        }

        public static void EnableDisplay()
        {
            Display.Enable();
        }

        public static void EnableWiFi()
        {
            Network.Enable();
        }

       

        private static void NetworkController_NetworkLinkConnectedChanged(NetworkController sender, NetworkLinkConnectedChangedEventArgs e)
        {
            IsLinkReady = e.Connected;
        }

        private static void NetworkController_NetworkAddressChanged(NetworkController sender, NetworkAddressChangedEventArgs e)
        {
            var ipProperties = sender.GetIPProperties();
            var address = ipProperties.Address.GetAddressBytes();
            IsNetworkConnected = address[0] != 0;

            if (IsNetworkConnected)
            {
                ListNetworkInfo(sender);
                ConnectNetworkTime();
                ConnectOpenWeather();
            }
        }

        public static void ConnectNetworkTime()
        {
            if (IsNetworkConnected == false)
            {
                return;
            }

            var timeServer = SettingsProvider.Settings.TimeServer;
            var timeOffset = SettingsProvider.Settings.TimeOffset;

            Debug.WriteLine(string.Empty);
            Debug.WriteLine("Synchronize local time with NTP:");
            Debug.WriteLine(string.Empty);
            Debug.WriteLine("   NTP Address . . . . . . . . . . . : " + timeServer);
            Debug.WriteLine("   Time Zone Offset. . . . . . . . . : " + timeOffset);
            Debug.WriteLine(string.Empty);

            try
            {
                DateTime currentTime = GetNtpTime(timeServer, timeOffset);

                if (currentTime == DateTime.MinValue)
                {
                    if (Rtc.IsValid)
                    {
                        Rtc.Now = currentTime;
                    }
                    SystemTime.SetTime(currentTime);
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
            if (IsNetworkConnected == false)
            {
                return;
            }

            var settings = SettingsProvider.Settings;
            var response = WeatherProvider.Connect(settings.Lat, settings.Lon, settings.Units, settings.OwmAppId);

            Debug.WriteLine("Retreive weather data from OpenWeather:");
            Debug.WriteLine("   Temperature . . . . . . . . . . . : " + response.Temp);
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
                Debug.WriteLine("   DHCP Enabled. . . . . . . . . . . : " + BoolToYesNo(activeSettings.IsDhcpEnabled));
                Debug.WriteLine("   IPv4 Address. . . . . . . . . . . : " + ip.Address.ToString());
                Debug.WriteLine("   Subnet Mask . . . . . . . . . . . : " + ip.SubnetMask.ToString());
                Debug.WriteLine("   Default Gateway . . . . . . . . . : " + ip.GatewayAddress.ToString());
                Debug.WriteLine(string.Empty);
                for (int i = 0; i < ip.DnsAddresses.Length; i++)
                {
                    Debug.WriteLine("   DNS Servers . . . . . . . . . . . : " + ip.DnsAddresses[i].ToString());
                }
                Debug.WriteLine("   Dynamic DNS Enabled . . . . . . . : " + BoolToYesNo(activeSettings.IsDynamicDnsEnabled));
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