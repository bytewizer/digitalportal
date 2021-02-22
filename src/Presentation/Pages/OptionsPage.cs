using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Input;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class OptionsPage : Page
    {
        public OptionsPage(int width, int height)
            : base(width, height)
        {
            ShowMenu = true;

            InitializePage();
        }

        public override void OnActivate() { }

        public override Panel CreatePageBody()
        {
            var panelWifi = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            panelWifi.TouchUp += PanelWifi_TouchUp;

            var textWifi = new DigitalText
            {
                Text = "Wireless",
                TextAlign = TextAlignment.Left
            };
            textWifi.SetMargin(5);
            textWifi.Width = 255 + 120;

            var buttonWifi = new Text()
            {
                TextContent = ResourcesProvider.UxNavigateNext,
                Font = ResourcesProvider.SmallUxIcons,
                ForeColor = SettingsProvider.Theme.Foreground,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Center,
                Width = 50
            };
            buttonWifi.SetMargin(5);
            
            panelWifi.Children.Add(textWifi);
            panelWifi.Children.Add(buttonWifi);

            var panelTheme = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            panelTheme.TouchUp += PanelTheme_TouchUp;

            var textTheme = new DigitalText
            {
                Text = "Theme Color",
                TextAlign = TextAlignment.Left
            };
            textTheme.SetMargin(5);
            textTheme.Width = 255 + 120;

            var buttonTheme = new Text()
            {
                TextContent = ResourcesProvider.UxNavigateNext,
                Font = ResourcesProvider.SmallUxIcons,
                ForeColor = SettingsProvider.Theme.Foreground,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Center,
                Width = 50
            };
            buttonTheme.SetMargin(5);
            
            panelTheme.Children.Add(textTheme);
            panelTheme.Children.Add(buttonTheme);

            var panelWeather = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            panelWeather.TouchUp += PanelWeather_TouchUp;

            var textWeather = new DigitalText
            {
                Text = "Weather Settings",
                TextAlign = TextAlignment.Left
            };
            textWeather.SetMargin(5);
            textWeather.Width = 255 + 120;

            var buttonWeather = new Text()
            {
                TextContent = ResourcesProvider.UxNavigateNext,
                Font = ResourcesProvider.SmallUxIcons,
                ForeColor = SettingsProvider.Theme.Foreground,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Center,
                Width = 50
            };
            buttonWeather.SetMargin(5);

            panelWeather.Children.Add(textWeather);
            panelWeather.Children.Add(buttonWeather);

            var panelAppearance = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            panelAppearance.TouchUp += PanelAppearance_TouchUp;

            var textApperance = new DigitalText
            {
                Text = "Apperance Settings",
                TextAlign = TextAlignment.Left
            };
            textApperance.SetMargin(5);
            textApperance.Width = 255 + 120;

            var buttonApperance = new Text()
            {
                TextContent = ResourcesProvider.UxNavigateNext,
                Font = ResourcesProvider.SmallUxIcons,
                ForeColor = SettingsProvider.Theme.Foreground,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Center,
                Width = 50
            };
            buttonApperance.SetMargin(5);

            panelAppearance.Children.Add(textApperance);
            panelAppearance.Children.Add(buttonApperance);

            PageBody.Children.Add(panelWifi);
            PageBody.Children.Add(panelTheme);
            PageBody.Children.Add(panelWeather);
            PageBody.Children.Add(panelAppearance);

            return PageBody;
        }

        private void PanelWifi_TouchUp(object sender, TouchEventArgs e)
        {
            Parent.Activate(5);
        }

        private void PanelTheme_TouchUp(object sender, TouchEventArgs e)
        {
            Parent.Activate(6);
        }

        private void PanelWeather_TouchUp(object sender, TouchEventArgs e)
        {
            Parent.Activate(7);
        }
        private void PanelAppearance_TouchUp(object sender, TouchEventArgs e)
        {
            Parent.Activate(8);
        }
    }
}