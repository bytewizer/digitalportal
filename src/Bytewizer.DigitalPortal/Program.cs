using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Hosting;

using Bytewizer.TinyCLR.DependencyInjection;

// To run this application in production please register for your own free
// API key from http://home.openweathermap.org. You can update this key 
// from the UX => Settings => Weather Settings => App Id or via source code in the "flashobject.cs" file.

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class Program
    {
        public static ApplicationService MainApplication;

        static void Main()
        {
            CreateHostBuilder().Build().RunGui();
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .UseDefaultServiceProvider(options =>
                {
                    options.ValidateOnBuild = true;
                })
                .ConfigureServices(services =>
                {
                    services.AddLogging(LogLevel.Trace);
                    services.AddThreadPool();

                    services.AddHostedService(typeof(WirelessWorker));
                    services.AddHostedService(typeof(WeatherWorker));
                    services.AddHostedService(typeof(DisplayWorker));

                    services.AddSingleton(typeof(ClockService));
                    services.AddSingleton(typeof(TouchService));
                    services.AddSingleton(typeof(BuzzerService));
                    services.AddSingleton(typeof(DisplayService));
                    services.AddSingleton(typeof(WirelessService));
                    services.AddSingleton(typeof(WeatherService));
                    services.AddSingleton(typeof(SettingsService));
                    services.AddSingleton(typeof(ApplicationService));

                    services.AddSingleton(typeof(MainWindow));
                    services.AddSingleton(typeof(IWindowPage), typeof(ClockPage));
                    services.AddSingleton(typeof(IWindowPage), typeof(WeatherPage));
                    services.AddSingleton(typeof(IWindowPage), typeof(AlarmPage));
                    services.AddSingleton(typeof(IWindowPage), typeof(OptionsPage));
                    services.AddSingleton(typeof(IWindowPage), typeof(NotAvailablePage));
                    services.AddSingleton(typeof(IWindowPage), typeof(WifiPage));
                    services.AddSingleton(typeof(IWindowPage), typeof(ThemePage));
                    services.AddSingleton(typeof(IWindowPage), typeof(OpenWeatherPage));
                    services.AddSingleton(typeof(IWindowPage), typeof(AppearancePage));
                });
    }
}



////if (!Memory.IsExtendedHeap())
////{
////    Memory.ExtendHeap();
////    Power.Reset();
////}

////Timer timer = new Timer(TimerTick, null, 3600000, 3600000); // 1 hour

//ClockProvider.Initialize();
//SettingsProvider.Initialize();
//BuzzerProvider.Initialize();
//DisplayProvider.Initialize();
//TouchProvider.Initialize();
//NetworkProvider.Initialize();
//WeatherProvider.Initialize();

//TouchProvider.Controller.TouchDown += Touch_TouchDown;
//TouchProvider.Controller.TouchUp += Touch_TouchUp;

//if (SettingsProvider.Flash.NetworkEnabled)
//{
//    NetworkProvider.EnableWifi();
//}

//MainProgram = new Application(DisplayProvider.Controller);

//var mainWindow = new MainWindow(DisplayProvider.Width, DisplayProvider.Height);

//var clockPage = new ClockPage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(clockPage);

//var weatherPage = new WeatherPage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(weatherPage);

//var alarmPage = new AlarmPage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(alarmPage);

//var optionsPage = new OptionsPage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(optionsPage);

//var notAvailablePage = new NotAvailablePage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(notAvailablePage);

//var wifiPage = new WifiPage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(wifiPage);

//var themePage = new ThemePage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(themePage);

//var weatherSettingsPage = new OpenWeatherPage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(weatherSettingsPage);

//var AppearancePage = new AppearancePage(DisplayProvider.Width, DisplayProvider.Height);
//mainWindow.Register(AppearancePage);

//mainWindow.Activate(SettingsProvider.Flash.DefaultPage);

//MainProgram.Run(mainWindow);


//private static void TimerTick(object sender)
//{
//    timerTick++;

//    if (NetworkProvider.IsConnected)
//    {
//        if (timerTick >= 24)
//        {
//            NetworkProvider.ConnectNetworkTime();
//            timerTick = 0;
//        }
//    }
//}

//private static void Touch_TouchUp(FT5xx6Controller sender, TouchEventArgs e) =>
//    MainProgram.InputProvider.RaiseTouch(e.X, e.Y, GHIElectronics.TinyCLR.UI.Input.TouchMessages.Up, DateTime.Now);

//private static void Touch_TouchDown(FT5xx6Controller sender, TouchEventArgs e) =>
//    MainProgram.InputProvider.RaiseTouch(e.X, e.Y, GHIElectronics.TinyCLR.UI.Input.TouchMessages.Down, DateTime.Now);