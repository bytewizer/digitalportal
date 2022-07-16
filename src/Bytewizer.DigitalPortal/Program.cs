using System;
using System.Threading;

using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;

// To run this application in production please register for your own free
// API key from http://home.openweathermap.org. You can update this key 
// from the UX => Settings => Weather Settings => App Id or via source code in the "flashobject.cs" file.

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class Program : Application
    {
        public static Program MainProgram;

        private static int timerTick = 1;

        public Program(DisplayController display)
            : base(display) { }

        static void Main()
        {
            if (!Memory.IsExtendedHeap())
            {
                Memory.ExtendHeap();
                Power.Reset();
            }

            Timer timer = new Timer(TimerTick, null, 3600000, 3600000); // 1 hour

            ClockProvider.Initialize();
            SettingsProvider.Initialize();
            
            //SettingsProvider.Flash.Ssid = "ssid";
            //SettingsProvider.Flash.Password = "password";
            //SettingsProvider.Flash.NetworkEnabled = false;
            //SettingsProvider.Flash.FlipOrientation = false;

            BuzzerProvider.Initialize();
            DisplayProvider.Initialize();
            TouchProvider.Initialize();
            NetworkProvider.Initialize();
            WeatherProvider.Initialize();

            TouchProvider.Controller.TouchDown += Touch_TouchDown;
            TouchProvider.Controller.TouchUp += Touch_TouchUp;

            if (SettingsProvider.Flash.NetworkEnabled)
            {
                NetworkProvider.EnableWifi();
            }

            MainProgram = new Program(DisplayProvider.Controller);

            var mainWindow = new MainWindow(DisplayProvider.Width, DisplayProvider.Height);

            var clockPage = new ClockPage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(clockPage);

            var weatherPage = new WeatherPage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(weatherPage);

            var alarmPage = new AlarmPage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(alarmPage);

            var optionsPage = new OptionsPage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(optionsPage);

            var notAvailablePage = new NotAvailablePage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(notAvailablePage);

            var wifiPage = new WifiPage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(wifiPage);

            var themePage = new ThemePage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(themePage);

            var weatherSettingsPage = new OpenWeatherPage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(weatherSettingsPage);

            var AppearancePage = new AppearancePage(DisplayProvider.Width, DisplayProvider.Height);
            mainWindow.Register(AppearancePage);

            mainWindow.Activate(SettingsProvider.Flash.DefaultPage);

            MainProgram.Run(mainWindow);
        }

        private static void TimerTick(object sender)
        {
            timerTick++;

            if (NetworkProvider.IsConnected)
            {
                if (timerTick >= 24)
                {
                    NetworkProvider.ConnectNetworkTime();
                    timerTick = 0;
                }
            }
        }

        private static void Touch_TouchUp(FT5xx6Controller sender, TouchEventArgs e) =>
            MainProgram.InputProvider.RaiseTouch(e.X, e.Y, GHIElectronics.TinyCLR.UI.Input.TouchMessages.Up, DateTime.Now);

        private static void Touch_TouchDown(FT5xx6Controller sender, TouchEventArgs e) =>
            MainProgram.InputProvider.RaiseTouch(e.X, e.Y, GHIElectronics.TinyCLR.UI.Input.TouchMessages.Down, DateTime.Now);
    }
}