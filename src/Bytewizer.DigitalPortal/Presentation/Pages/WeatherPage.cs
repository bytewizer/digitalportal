using System;

using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Shapes;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Threading;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    class WeatherPage : Page
    {
        private readonly ClockService _clock;
        private readonly WeatherService _weather;

        private readonly DispatcherTimer dateTimer;
        private readonly DispatcherTimer weatherTimer;

        private DigitalText textLocation;
        private DigitalText textDate;

        private DigitalText textTemp;
        private DigitalText textIcon;
        private DigitalText textHigh;
        private DigitalText textLow;
        private DigitalText textDescription;

        private DigitalText textWind;
        private DigitalText textHumidity;

        private DigitalText[] textForcastDate;
        private DigitalText[] textForcastIcon;
        private DigitalText[] textForcastHigh;
        private DigitalText[] textForcastLow;

        public WeatherPage(DisplayService display, ClockService clock, WeatherService weather)
            : base(display.Width, display.Height)
        {
            _clock = clock;
            _weather = weather;

            ShowMenu = true;

            dateTimer = new DispatcherTimer();
            weatherTimer = new DispatcherTimer();

            dateTimer.Tick += ClockTimer_Tick;
            dateTimer.Interval = new TimeSpan(0, 0, 1);
            dateTimer.Start();

            weatherTimer.Tick += WeatherTimer_Tick;
            weatherTimer.Interval = new TimeSpan(0, 10, 0);
            weatherTimer.Start();

            InitializePage();
        }

        public override void OnActivate() 
        {
        }

        public override Panel CreatePageBody()
        {
            var panelHeader = new StackPanel(Orientation.Horizontal)
            {
                VerticalAlignment = VerticalAlignment.Top
            };

            var panelCurrent = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var panelForcast = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var panelWeather = new StackPanel(Orientation.Horizontal)
            {
            };
            panelWeather.Width = 280 - 20;

            var panelInfo = new StackPanel(Orientation.Vertical)
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            panelInfo.Width = 200 - 20;

            var panelHighLow = new StackPanel(Orientation.Vertical)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            panelHighLow.SetMargin(10);

            textLocation = new DigitalText
            {
                Text = SettingsService.Flash.Location
            };
            textLocation.SetMargin(5);
            textLocation.Width = 200 - 10;
            panelHeader.Children.Add(textLocation);

            textDate = new DigitalText
            {
                Text = _clock.LocalTime.ToString("ddd, MMM dd hh:mm tt"),
                TextAlign = TextAlignment.Right
            };
            textDate.SetMargin(5);
            textDate.Width = 280 - 10;
            panelHeader.Children.Add(textDate);

            textIcon = new DigitalText
            {
                Text = SettingsService.Weather.Icon,
                Font = ResourcesProvider.MediumWeatherIcons,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted)
            };
            textIcon.SetMargin(0, 0, 2, 0);

            textTemp = new DigitalText
            {
                Text = SettingsService.Weather.Temp,
                Font = ResourcesProvider.MediumDigitalFont,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                Width = 80
            };

            textHigh = new DigitalText
            {
                Text = SettingsService.Weather.High,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 40
            };
            textHigh.SetMargin(0, 0, 0, 2);

            var lineHighLow = new Line(20, 0)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Stroke = new Pen(SettingsService.Theme.Shadow)
            };

            textLow = new DigitalText
            {
                Text = SettingsService.Weather.Low,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 40
            };
            textLow.SetMargin(0, 2, 0, 0);

            var textUnit = new DigitalText
            {
                Text = SettingsService.Weather.TempUnit,
                Font = ResourcesProvider.SmallWeatherIcons,
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted)
            };
            textUnit.SetMargin(0, 4, 0, 0);

            panelHighLow.Children.Add(textHigh);
            panelHighLow.Children.Add(lineHighLow);
            panelHighLow.Children.Add(textLow);

            panelWeather.Children.Add(textIcon);
            panelWeather.Children.Add(textTemp);
            panelWeather.Children.Add(panelHighLow);
            panelWeather.Children.Add(textUnit);

            textWind = new DigitalText
            {
                Text = SettingsService.Weather.Wind,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                TextAlign = TextAlignment.Left
            };

            textHumidity = new DigitalText
            {
                Text = SettingsService.Weather.Humidity,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                TextAlign = TextAlignment.Left
            };

            textDescription = new DigitalText
            {
                Text = SettingsService.Weather.Description
            };
            textDescription.Width = 480 - 40;
            textDescription.SetMargin(5);

            panelInfo.Children.Add(textWind);
            panelInfo.Children.Add(textHumidity);

            panelCurrent.Children.Add(panelWeather);
            panelCurrent.Children.Add(panelInfo);

            var line = new Line(460, 0)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Stroke = new Pen(SettingsService.Theme.Shadow)
            };

            var date = new string[5];
            var icon = new string[5];
            var high = new int[5];
            var low = new int[5];
            var forcastLine = new Line[5];

            var panelDay = new StackPanel[date.Length];

            textForcastDate = new DigitalText[date.Length];
            textForcastIcon = new DigitalText[icon.Length];
            textForcastHigh = new DigitalText[low.Length];
            textForcastLow = new DigitalText[high.Length];

            var forcastWidth = (Width - 20) / 5;

            for (var i = 0; i < date.Length; i++)
            {
                panelDay[i] = new StackPanel(Orientation.Vertical)
                {
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                panelDay[i].Width = forcastWidth;

                textForcastDate[i] = new DigitalText()
                {
                    Text = SettingsService.Weather.Forcast[i].Date,
                    Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                    TextAlign = TextAlignment.Center
                };
                textForcastDate[i].SetMargin(2);
                panelDay[i].Children.Add(textForcastDate[i]);

                var panelFiveDayForcast = new StackPanel(Orientation.Horizontal)
                {

                };

                var panelIcon = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                panelIcon.Width = forcastWidth / 2;

                textForcastIcon[i] = new DigitalText
                {
                    Text = SettingsService.Weather.Forcast[i].Icon,
                    Font = ResourcesProvider.SmallWeatherIcons,
                    Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                    TextAlign = TextAlignment.Right
                };
                panelIcon.Children.Add(textForcastIcon[i]);
                panelFiveDayForcast.Children.Add(panelIcon);

                var panelForcastHighLow = new StackPanel(Orientation.Vertical)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                panelForcastHighLow.Width = forcastWidth / 2;

                textForcastHigh[i] = new DigitalText
                {
                    Text = SettingsService.Weather.Forcast[i].High,
                    TextAlign = TextAlignment.Center
                };
                textForcastHigh[i].SetMargin(0, 0, 0, 2);
                panelForcastHighLow.Children.Add(textForcastHigh[i]);

                forcastLine[i] = new Line(20, 0)
                {
                    Stroke = new Pen(SettingsService.Theme.Shadow)
                };
                panelForcastHighLow.Children.Add(forcastLine[i]);

                textForcastLow[i] = new DigitalText
                {
                    Text = SettingsService.Weather.Forcast[i].Low,
                    TextAlign = TextAlignment.Center
                };
                textForcastLow[i].SetMargin(0, 2, 0, 0);
                panelForcastHighLow.Children.Add(textForcastLow[i]);

                panelFiveDayForcast.Children.Add(panelForcastHighLow);
                panelDay[i].Children.Add(panelFiveDayForcast);

                panelForcast.Children.Add(panelDay[i]);
            }

            PageBody.Children.Add(panelHeader);
            PageBody.Children.Add(panelCurrent);
            PageBody.Children.Add(textDescription);
            PageBody.Children.Add(line);
            PageBody.Children.Add(panelForcast);

            return PageBody;
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            var dateTime = _clock.LocalTime;
            UXExtensions.DoThreadSafeAction(textDate, () =>
            {
                textDate.Text = dateTime.ToString("ddd, MMM dd hh:mm tt");
                textDate.Invalidate();
            });

            UXExtensions.DoThreadSafeAction(textLocation, () =>
            {
                textLocation.Text = SettingsService.Flash.Location;
                textLocation.Invalidate();
            });
        }

        private void WeatherTimer_Tick(object sender, EventArgs e)
        {
            GetWeather();
        }

        private void GetWeather()
        {
            if (SettingsService.Weather == null)
            {
                return;
            }

            _weather.Connect();
            var data = SettingsService.Weather;

            UXExtensions.DoThreadSafeAction(textTemp, () =>
            {
                textTemp.Text = data.Temp;
                textTemp.Invalidate();
            });

            UXExtensions.DoThreadSafeAction(textIcon, () =>
            {
                textIcon.Text = data.Icon;
                textIcon.Invalidate();
            });

            UXExtensions.DoThreadSafeAction(textHigh, () =>
            {
                textHigh.Text = data.High;
                textHigh.Invalidate();
            });

            UXExtensions.DoThreadSafeAction(textLow, () =>
            {
                textLow.Text = data.Low;
                textLow.Invalidate();
            });

            UXExtensions.DoThreadSafeAction(textDescription, () =>
            {
                textDescription.Text = data.Description;
                textDescription.Invalidate();
            });

            UXExtensions.DoThreadSafeAction(textWind, () =>
            {
                //textWind.Text = string.Format($"wind: { data.Wind } mph {data.WindDirection}");
                textWind.Text = data.Wind;
                textWind.Invalidate();
            });

            UXExtensions.DoThreadSafeAction(textHumidity, () =>
            {
                //textHumidity.Text = string.Format($"humidity: { data.Humidity }%");
                textHumidity.Text = data.Humidity;
                textHumidity.Invalidate();
            });

            for (var i = 0; i < data.Forcast.Length; i++)
            {
                UXExtensions.DoThreadSafeAction(textForcastDate[i], () =>
                {
                    textForcastDate[i].Text = data.Forcast[i].Date;
                    textForcastDate[i].Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textForcastIcon[i], () =>
                {
                    textForcastIcon[i].Text = data.Forcast[i].Icon;
                    textForcastIcon[i].Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textForcastHigh[i], () =>
                {
                    textForcastHigh[i].Text = data.Forcast[i].High;
                    textForcastHigh[i].Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textForcastLow[i], () =>
                {
                    textForcastLow[i].Text = data.Forcast[i].Low;
                    textForcastLow[i].Invalidate();
                });
            }
        }
    }
}