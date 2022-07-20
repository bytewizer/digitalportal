using System;
using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.DependencyInjection;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    /// <summary>
    /// Extensions for <see cref="IHost"/>.
    /// </summary>
    public static class HostingAbstractionsHostExtensions
    {
        /// <summary>
        /// Runs a gui application and block the calling thread.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/> to run.</param>
        public static void RunGui(this IHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException();
            }

            host.Start();

            var mainWindow = (MainWindow)host.Services.GetRequiredService(typeof(MainWindow));
            var pages = host.Services.GetServices(typeof(IWindowPage));

            foreach(IWindowPage page in pages)
            {
                mainWindow.Register(page);
            }

            mainWindow.Activate(SettingsService.Flash.DefaultPage);
            var application = (ApplicationService)host.Services.GetRequiredService(typeof(ApplicationService));
            
            application.Run(mainWindow);
        }
    }
}


//            //var mainWindow = new MainWindow(_display);

//            //var clockPage = new ClockPage(_display, _clock, _network);
//            //mainWindow.Register(clockPage);

//            //var weatherPage = new WeatherPage(_display, _clock);
//            //mainWindow.Register(weatherPage);

//            //var alarmPage = new AlarmPage(_display);
//            //mainWindow.Register(alarmPage);

//            //var optionsPage = new OptionsPage(_display);
//            //mainWindow.Register(optionsPage);

//            //var notAvailablePage = new NotAvailablePage(_display);
//            //mainWindow.Register(notAvailablePage);

//            //var wifiPage = new WifiPage(_display, _network, _settings);
//            //mainWindow.Register(wifiPage);

//            //var themePage = new ThemePage(_display, _settings);
//            //mainWindow.Register(themePage);

//            //var weatherSettingsPage = new OpenWeatherPage(_display, _network, _settings);
//            //mainWindow.Register(weatherSettingsPage);

//            //var AppearancePage = new AppearancePage(_display, _network, _settings);
//            //mainWindow.Register(AppearancePage);

//            //mainWindow.Activate(_settings.Flash.DefaultPage);

//            //MainWindow = mainWindow;
