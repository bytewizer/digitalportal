using System;
using System.Collections;

using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Input;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Shapes;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Threading;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public sealed class MainWindow : Window
    {
        private ArrayList WindowPages { get; set; }

        private readonly DispatcherTimer menuTimer;

        private readonly Panel panelRoot = new Panel();

        private int activePage;

        public MainWindow(int width, int height)
        {
            Width = width;
            Height = height;
            WindowPages = new ArrayList();

            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            Foreground = new SolidColorBrush(Color.FromArgb(0, 3, 255, 14));
            Visibility = Visibility.Visible;

            menuTimer = new DispatcherTimer();
            menuTimer.Tick += MenuTimer_Tick;
        }

        public void Register(IWindowPage page)
        {
            page.Parent = this;
            page.Id = WindowPages.Count;

            if (page.ShowMenu == true)
            {
                page.Child.TouchUp += MenuBar_TouchUp;
            }

            WindowPages.Add(page);
        }

        private void MenuBar_TouchUp(object sender, TouchEventArgs e)
        {
            Activate();
            menuTimer.Interval = new TimeSpan(0, 0, 5);
            menuTimer.Start();
        }

        public void Refresh()
        {
            foreach (IWindowPage window in WindowPages)
            {
                window.InitializePage();
            }
        }

        public void Activate()
        {
            Activate(activePage, false);
        }

        public void Activate(int page)
        {
            Activate(page, false);
        }

        public void Activate(int page, bool refresh)
        {
            activePage = page;

            var window = (IWindowPage)WindowPages[page];

            if (refresh == true)
            {
                window.InitializePage();
            }

            var body = window.Child;

            var menu = CreateMenuBar();

            if (window.ShowMenu == true)
            {
                menu.Visibility = Visibility.Visible;
            }
            else
            {
                menu.Visibility = Visibility.Hidden;
            }

            menuTimer.Tag = menu;

            panelRoot.Children.Clear();
            panelRoot.Children.Add(body);
            panelRoot.Children.Add(menu);

            Child = panelRoot;

            window.OnActivate();
        }

        public Panel CreateMenuBar()
        {
            var menuBarHeight = 40;

            var panelMenu = new StackPanel(Orientation.Horizontal)
            {
                VerticalAlignment = VerticalAlignment.Bottom
            };
            panelMenu.TouchUp += PanelMenu_TouchUp;

            var panelBar = new Panel
            {
                Height = menuBarHeight
            };

            var rectangleMenu = new Rectangle()
            {
                Width = ActualWidth,
                Height = menuBarHeight,
                Fill = new SolidColorBrush(ThemeColors.Menu),
                Stroke = new Pen(ThemeColors.Menu),
            };
            panelBar.Children.Add(rectangleMenu);

            var panelIcons = new StackPanel(Orientation.Horizontal)
            {
            };

            var buttonWidth = ActualWidth / 4;

            // Clock
            var panelAccessTime = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            panelAccessTime.Width = buttonWidth;
            panelAccessTime.TouchUp += PanelAccessTime_TouchUp;

            var iconAccessTime = new Text()
            {
                TextContent = ResourcesProvider.UxAccessTime,
                Font = ResourcesProvider.SmallUxIcons,
                TextAlignment = TextAlignment.Center,
            };
            SetActive(iconAccessTime, 0);
            panelAccessTime.Children.Add(iconAccessTime);
            panelIcons.Children.Add(panelAccessTime);

            // Weather
            var panelCloud = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            panelCloud.Width = buttonWidth;
            panelCloud.TouchUp += PanelCloud_TouchUp;

            var iconCloud = new Text()
            {
                TextContent = ResourcesProvider.UxCloud,
                Font = ResourcesProvider.SmallUxIcons,
                TextAlignment = TextAlignment.Center,
            };
            SetActive(iconCloud, 1);
            panelCloud.Children.Add(iconCloud);
            panelIcons.Children.Add(panelCloud);

            // Alarm
            var panelNotification = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            panelNotification.Width = buttonWidth;
            panelNotification.TouchUp += PanelNotification_TouchUp;

            var iconNotification = new Text()
            {
                TextContent = ResourcesProvider.UxNotification,
                Font = ResourcesProvider.SmallUxIcons,
                TextAlignment = TextAlignment.Center,
            };
            SetActive(iconNotification, 2);
            panelNotification.Children.Add(iconNotification);
            panelIcons.Children.Add(panelNotification);

            // Settings
            var panelSettings = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            panelSettings.Width = buttonWidth;
            panelSettings.TouchUp += PanelSettings_TouchUp;

            var iconSettings = new Text()
            {
                TextContent = ResourcesProvider.UxSettings,
                Font = ResourcesProvider.SmallUxIcons,
                TextAlignment = TextAlignment.Center,
            };
            SetActive(iconSettings, 3);
            panelSettings.Children.Add(iconSettings);
            panelIcons.Children.Add(panelSettings);

            panelBar.Children.Add(panelIcons);
            panelMenu.Children.Add(panelBar);

            return panelMenu;
        }

        private void MenuTimer_Tick(object sender, EventArgs e)
        {
            var panelMenu = (StackPanel)((DispatcherTimer)sender).Tag;
            UXExtensions.DoThreadSafeAction(panelMenu, () =>
            {
                if (panelMenu.Visibility == Visibility.Visible)
                {
                    panelMenu.Visibility = Visibility.Hidden;
                    panelMenu.Invalidate();
                    Invalidate();
                }
            });
            menuTimer.Stop();
        }

        private void PanelMenu_TouchUp(object sender, TouchEventArgs e)
        {
            menuTimer.Interval = new TimeSpan(0, 0, 5);

            //BuzzerProvider.Play(
            //    new MusicNote(Tone.G3, 35),
            //    new MusicNote(Tone.G4, 35),
            //    new MusicNote(Tone.G5, 35)
            //    );
        }

        private void PanelAccessTime_TouchUp(object sender, TouchEventArgs e)
        {
            Activate(0);
        }

        private void PanelCloud_TouchUp(object sender, TouchEventArgs e)
        {
            if (NetworkProvider.IsConnected)
            { 
                Activate(1, true); 
            }
            else
            { 
                Activate(4);
            }
        }

        private void PanelNotification_TouchUp(object sender, TouchEventArgs e)
        {
            Activate(2);
        }

        private void PanelSettings_TouchUp(object sender, TouchEventArgs e)
        {
            Activate(3);
        }

        private void SetActive(Text element, int page)
        {
            if (activePage == page)
            {
                element.ForeColor = SettingsProvider.Theme.Highlighted;
            }
            else
            {
                element.ForeColor = SettingsProvider.Theme.Shadow;
            }
        }
    }
}