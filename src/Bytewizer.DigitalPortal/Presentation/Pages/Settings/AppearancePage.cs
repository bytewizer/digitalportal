using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class AppearancePage : SettingPage
    {
        private ToggleSwitch toggleOrientation;
        private ToggleSwitch toggleShowDow;

        public AppearancePage(int width, int height)
            : base(width, height)
        {
            BackText = ResourcesProvider.UxNavigateBefore;
            Title = "appearance settings";

            BackClick += AppearancePage_BackClick;
            InitializePage();
        }

        public override void CreateBody()
        {
            var panelOrientation = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 40
            };

            var labelOrientation = new Text()
            {
                TextContent = "Flip Orientation",
                Font = ResourcesProvider.SmallDigitalFont,
                ForeColor = SettingsProvider.Theme.Standard,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Left,
                Width = 480 - 40 - 60
            };

            toggleOrientation = new ToggleSwitch()
            {
                IsOn = SettingsProvider.Flash.FlipOrientation,
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Highlighted),
                Background = new SolidColorBrush(SettingsProvider.Theme.Shadow),
                Width = 60,
                Height = 40
            };
            toggleOrientation.Click += ToggleOrientation_Click;

            panelOrientation.Children.Add(labelOrientation);
            panelOrientation.Children.Add(toggleOrientation);

            var panelShowDow = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 40
            };

            var labelShowDow = new Text()
            {
                TextContent = "Show Days of the Week",
                Font = ResourcesProvider.SmallDigitalFont,
                ForeColor = SettingsProvider.Theme.Standard,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Left,
                Width = 480 - 40 - 60
            };

            toggleShowDow = new ToggleSwitch()
            {
                IsOn = SettingsProvider.Flash.FlipOrientation,
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Highlighted),
                Background = new SolidColorBrush(SettingsProvider.Theme.Shadow),
                Width = 60,
                Height = 40
            };
            toggleShowDow.Click += ToggleShowDow_Click;

            panelShowDow.Children.Add(labelShowDow);
            panelShowDow.Children.Add(toggleShowDow);

            PageBody.Children.Add(panelOrientation);
            PageBody.Children.Add(panelShowDow);
        }

        public override void OnActivate() 
        {
            toggleOrientation.IsOn = SettingsProvider.Flash.FlipOrientation;
            toggleShowDow.IsOn = SettingsProvider.Flash.ShowDow;
        }

        private void AppearancePage_BackClick(object sender, RoutedEventArgs e)
        {
            Parent.Refresh();
            Parent.Activate(3);
        }

        private void ToggleOrientation_Click(object sender, RoutedEventArgs e)
        {
            var orientation = ((ToggleSwitch)sender).IsOn;

            if (orientation == true)
            {
                SettingsProvider.WriteOrientation(false);
            }
            else
            {
                SettingsProvider.WriteOrientation(true);
            }

            NetworkProvider.DisableWifi();
            
            Power.Reset(); 
        }
        
        private void ToggleShowDow_Click(object sender, RoutedEventArgs e)
        {
            var dow = ((ToggleSwitch)sender).IsOn;

            if (dow)
            {
                SettingsProvider.WriteShowDow(false);
            }
            else
            {
                SettingsProvider.WriteShowDow(true);
            }

            UXExtensions.DoThreadSafeAction(toggleShowDow, () =>
            {
                toggleShowDow.IsOn = dow;
                toggleShowDow.Invalidate();
            });

            Parent.Invalidate();
        }
    }
}