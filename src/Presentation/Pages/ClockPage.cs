using System;

using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Threading;
using GHIElectronics.TinyCLR.Drivers.Microchip.Winc15x0;
using System.Diagnostics;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    class ClockPage : Page
    {
        private readonly DispatcherTimer clockTimer;
        private readonly DispatcherTimer statusTimer;

        private int statusTick;

        private DigitalText textAlarmIcon;
        private DigitalText textAlarmStatus;
        private DigitalText textStatus;
        private DigitalText textWifi;
        private DigitalText textTime;
        private DigitalText textMeridiem;
        private DigitalText textSeconds;
        private DigitalText textSeparator;
        private DigitalText[] textDays;

        public ClockPage(int width, int height)
            : base(width, height)
        {
            ShowMenu = true;

            clockTimer = new DispatcherTimer();
            statusTimer = new DispatcherTimer();

            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Start();

            statusTimer.Tick += StatusTimer_Tick;
            statusTimer.Interval = new TimeSpan(0, 0, 1);
            statusTimer.Start();

            InitializePage();
        }
        
        public override void OnActivate() { }

        public override Panel CreatePageBody()
        {
            CreateBody();
            return PageBody;
        }

        public void CreateBody()
        {
            var panelHeader = new StackPanel(Orientation.Horizontal)
            {
                VerticalAlignment = VerticalAlignment.Top
            };

            var panelTime = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var panelSeconds = new StackPanel(Orientation.Horizontal);

            var panelMeridiem = new StackPanel(Orientation.Vertical)
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            var panelDoW = new StackPanel(Orientation.Horizontal)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            textAlarmIcon = new DigitalText
            {
                Text = ResourcesProvider.UxNotificationOff,
                Font = ResourcesProvider.SmallUxIcons,
                Width = 20
            };
            textAlarmIcon.SetMargin(5);

            textAlarmStatus = new DigitalText
            {
                Text = "no alarm",
                Width = 90
            };
            textAlarmStatus.SetMargin(5);

            textStatus = new DigitalText
            {
                TextAlign = TextAlignment.Right,
                Width = 300
            };
            textStatus.SetMargin(5);

            textWifi = new DigitalText
            {
                Font = ResourcesProvider.SmallUxIcons,
                TextAlign = TextAlignment.Right,
                Width = 30
            };
            textAlarmIcon.SetMargin(5);
            textWifi.Visibility = Visibility.Visible;

            panelHeader.Children.Add(textAlarmIcon);
            panelHeader.Children.Add(textAlarmStatus);
            panelHeader.Children.Add(textStatus);
            panelHeader.Children.Add(textWifi);

            textTime = new DigitalText
            {
                Text = "00:00",
                Font = ResourcesProvider.LargeDigitalFont,
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Highlighted)
            };

            textMeridiem = new DigitalText
            {
                Text = "AM",
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Highlighted),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            textSeparator = new DigitalText
            {
                Text = ":",
                Font = ResourcesProvider.MediumDigitalFont,
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Highlighted)
            };

            textSeconds = new DigitalText
            {
                Text = "00",
                Font = ResourcesProvider.MediumDigitalFont,
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Highlighted)
            };

            var daysofweek = new string[] { "sun", "mon", "tue", "wed", "thr", "fri", "sat" };
            textDays = new DigitalText[daysofweek.Length];

            for (var i = 0; i < daysofweek.Length; i++)
            {
                textDays[i] = new DigitalText
                {
                    Text = daysofweek[i],
                    Foreground = new SolidColorBrush(SettingsProvider.Theme.Shadow)
                };
                textDays[i].SetMargin(12);
            }

            foreach (var element in textDays)
            {
                panelDoW.Children.Add(element);
            }

            panelSeconds.Children.Add(textSeparator);
            panelSeconds.Children.Add(textSeconds);

            panelMeridiem.Children.Add(textMeridiem);
            panelMeridiem.Children.Add(panelSeconds);
            
            panelTime.Children.Add(textTime);
            panelTime.Children.Add(panelMeridiem);

            PageBody.Children.Add(panelHeader);
            PageBody.Children.Add(panelTime);
            
            if (SettingsProvider.Flash.ShowDow == true)
            {
                PageBody.Children.Add(panelDoW);
            }

            PageBody.Visibility = Visibility.Hidden;
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            statusTick++;

            if (statusTick == 1)
            {
                if (SettingsProvider.Flash.ShowDate == false)
                {
                    return;
                }

                var dateTime = ClockProvider.Controller.Now;
                UXExtensions.DoThreadSafeAction(textStatus, () =>
                {
                    textStatus.Text = dateTime.ToString("MMM dd yyy");
                    textStatus.Invalidate();
                });

                return;
            }

            if (NetworkProvider.IsConnected)
            {
                if (SettingsProvider.Flash.ShowWeather == false)
                {
                    return;
                }
                
                if (statusTick == 7)
                {
                    var data = WeatherProvider.Weather;

                    UXExtensions.DoThreadSafeAction(textStatus, () =>
                    {
                        textStatus.Text = string.Format($"now:{data.Temp} {data.Description}");
                        textStatus.Invalidate();
                    });

                    return;
                }

                if (statusTick == 12)
                {
                    var data = WeatherProvider.Weather;

                    UXExtensions.DoThreadSafeAction(textStatus, () =>
                    {
                        var high = data.High;
                        var low = data.Low;
                        textStatus.Text = string.Format($"today: {high}/{low}");
                        textStatus.Invalidate();
                    });

                }
            }

            if (statusTick == 17)
            {
                statusTick = 0;
            }
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            var dateTime = ClockProvider.Controller.Now;

            UXExtensions.DoThreadSafeAction(textTime, () =>
            {
                textTime.Text = dateTime.ToString("hh:mm"); ;
                textTime.Invalidate();
            });

            UXExtensions.DoThreadSafeAction(textMeridiem, () =>
            {
                textMeridiem.Text = dateTime.ToString("tt"); ;
                textMeridiem.Invalidate();
            });

            if (textSeparator.Visibility == Visibility.Visible)
            {
                UXExtensions.DoThreadSafeAction(textSeparator, () =>
                {
                    textSeparator.Visibility = Visibility.Hidden;
                    textSeparator.Invalidate();
                });
            }
            else
            {
                UXExtensions.DoThreadSafeAction(textSeparator, () =>
                {
                    textSeparator.Visibility = Visibility.Visible;
                    textSeparator.Invalidate();
                });
            }

            UXExtensions.DoThreadSafeAction(textSeconds, () =>
            {
                textSeconds.Text = dateTime.ToString("ss");
                textSeconds.Invalidate();
            });

            for (var i = 0; i < textDays.Length; i++)
            {
                if (i == (int)dateTime.DayOfWeek)
                {
                    UXExtensions.DoThreadSafeAction(textDays[i], () =>
                    {
                        textDays[i].Foreground = new SolidColorBrush(SettingsProvider.Theme.Highlighted);
                        textDays[i].Invalidate();
                    });
                }
                else
                {
                    UXExtensions.DoThreadSafeAction(textDays[i], () =>
                    {
                        textDays[i].Foreground = new SolidColorBrush(SettingsProvider.Theme.Shadow);
                        textDays[i].Invalidate();
                    });
                }
            }

            if (NetworkProvider.IsConnected)
            {
                UXExtensions.DoThreadSafeAction(textWifi, () =>
                {
                    textWifi.Text = ResourcesProvider.UxWiFi;
                    textWifi.Invalidate();
                });
            }
            else
            {
                UXExtensions.DoThreadSafeAction(textWifi, () =>
                {
                    textWifi.Text = ResourcesProvider.UxWiFiOff;
                    textWifi.Invalidate();
                });
            }

            if (PageBody.Visibility == Visibility.Hidden)
            {
                PageBody.Visibility = Visibility.Visible;
            }
        }
    }
}