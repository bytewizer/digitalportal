using System;

using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Threading;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    class ClockPage : Page
    {
        private readonly ClockService _clock;
        
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

        public ClockPage(DisplayService display, ClockService clock)
            : base(display.Width, display.Height)
        {
            _clock = clock;

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
                Width = 25
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
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                Width = 300
            };

            textMeridiem = new DigitalText
            {
                Text = "AM",
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 25             
            };

            textSeparator = new DigitalText
            {
                Text = ":",
                Font = ResourcesProvider.MediumDigitalFont,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted)
            };

            textSeconds = new DigitalText
            {
                Text = "00",
                Font = ResourcesProvider.MediumDigitalFont,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                Width = 80
            };

            var daysofweek = new string[] { "sun", "mon", "tue", "wed", "thr", "fri", "sat" };
            textDays = new DigitalText[daysofweek.Length];

            for (var i = 0; i < daysofweek.Length; i++)
            {
                textDays[i] = new DigitalText
                {
                    Text = daysofweek[i],
                    Foreground = new SolidColorBrush(SettingsService.Theme.Shadow),
                    Width = 35
                    
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
            
            if (SettingsService.Flash.ShowDow == true)
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
                if (SettingsService.Flash.ShowDate == false)
                {
                    return;
                }

                var dateTime = _clock.LocalTime;
                UXExtensions.DoThreadSafeAction(textStatus, () =>
                {
                    textStatus.Text = dateTime.ToString("MMM dd yyy");
                    textStatus.Invalidate();
                });

                return;
            }

            if (SettingsService.NetworkConnected)
            {
                if (SettingsService.Flash.ShowWeather == false)
                {
                    return;
                }
                
                if (statusTick == 7)
                {
                    var data = SettingsService.Weather;

                    UXExtensions.DoThreadSafeAction(textStatus, () =>
                    {
                        textStatus.Text = string.Format($"now:{data.Temp} {data.Description}");
                        textStatus.Invalidate();
                    });

                    return;
                }

                if (statusTick == 12)
                {
                    var data = SettingsService.Weather;

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
            var dateTime = _clock.LocalTime;

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
                        textDays[i].Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted);
                        textDays[i].Invalidate();
                    });
                }
                else
                {
                    UXExtensions.DoThreadSafeAction(textDays[i], () =>
                    {
                        textDays[i].Foreground = new SolidColorBrush(SettingsService.Theme.Shadow);
                        textDays[i].Invalidate();
                    });
                }
            }

            if (SettingsService.NetworkConnected)
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