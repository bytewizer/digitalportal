using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WifiPage : SettingPage
    {
        private SettingsService _settings;

        private TextBox textNetwork;
        private TextBox textPassword;
        private ToggleSwitch textWiFi;

        public WifiPage(
            DisplayService display,
            SettingsService settings)

            : base(display.Width, display.Height)
        {
            //_network = network;
            _settings = settings;

            BackText = ResourcesProvider.UxNavigateBefore;
            Title = "Wireless";

            BackClick += WifiPage_BackClick;

            InitializePage();
        }

        public override void CreateBody()
        {
            SetPageColor(SettingsService.Theme.Standard);

            OnScreenKeyboard.Font = ResourcesProvider.SmallRobotoFont;

            var panelWiFi = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 40
            };

            var labelWiFi = new Text()
            {
                TextContent = "Wireless",
                Font = ResourcesProvider.SmallDigitalFont,
                ForeColor = SettingsService.Theme.Standard,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Left,
                Width = 480 - 40 - 60
            };

            textWiFi = new ToggleSwitch()
            {
                IsOn = SettingsService.Flash.NetworkEnabled,
                Foreground = new SolidColorBrush(SettingsService.Theme.Highlighted),
                Background = new SolidColorBrush(SettingsService.Theme.Shadow),
                Width = 60,
                Height = 40
            };
            textWiFi.Click += TextWiFi_Click;

            panelWiFi.Children.Add(labelWiFi);
            panelWiFi.Children.Add(textWiFi);

            var panelNetwork = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 40
            };

            var labelNetwork = new Text()
            {
                TextContent = "Network",
                Font = ResourcesProvider.SmallDigitalFont,
                ForeColor = SettingsService.Theme.Standard,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Left,
                Width = 150
            };

            textNetwork = new TextBox()
            {
                Text = SettingsService.Flash.Ssid,
                Font = ResourcesProvider.SmallRobotoFont,
                Foreground = new SolidColorBrush(SettingsService.Theme.Standard),
                Background = new SolidColorBrush(SettingsService.Theme.Shadow),
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlign = TextAlignment.Center,
                Width = 290
            };
            textNetwork.TextChanged += TextNetwork_TextChanged;

            panelNetwork.Children.Add(labelNetwork);
            panelNetwork.Children.Add(textNetwork);

            var panelPassword = new StackPanel(Orientation.Horizontal)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 40
            };

            var labelPassword = new Text()
            {
                TextContent = "Password",
                Font = ResourcesProvider.SmallDigitalFont,
                ForeColor = SettingsService.Theme.Standard,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Left,
                Width = 150
            };

            textPassword = new TextBox()
            {
                Text = SettingsService.Flash.Password,
                Font = ResourcesProvider.SmallRobotoFont,
                Foreground = new SolidColorBrush(SettingsService.Theme.Standard),
                Background = new SolidColorBrush(SettingsService.Theme.Shadow),
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlign = TextAlignment.Center,
                Width = 290
            };
            textPassword.TextChanged += TextPassword_TextChanged;

            panelPassword.Children.Add(labelPassword);
            panelPassword.Children.Add(textPassword);

            PageBody.Children.Add(panelWiFi);
            PageBody.Children.Add(panelNetwork);
            PageBody.Children.Add(panelPassword);
        }

        public override void OnActivate() 
        {
            if (SettingsService.NetworkConnected)
            {
                UXExtensions.DoThreadSafeAction(textNetwork, () =>
                {
                    textNetwork.IsEnabled = false;
                    textNetwork.Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textPassword, () =>
                {
                    textPassword.IsEnabled = false;
                    textPassword.Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textWiFi, () =>
                {
                    textWiFi.IsOn = true;
                    textWiFi.Invalidate();
                });
            }
        }

        private void WifiPage_BackClick(object sender, RoutedEventArgs e)
        {
            Parent.Activate(3);
        }

        private void TextWiFi_Click(object sender, RoutedEventArgs e)
        {
            var networkEnabled = ((ToggleSwitch)sender).IsOn;

            if (networkEnabled == true)
            {
                //_network.Controller.Disable();
                _settings.WriteWifiEnabled(false);

                UXExtensions.DoThreadSafeAction(textWiFi, () =>
                {
                    textWiFi.IsOn = true;
                    textWiFi.Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textNetwork, () =>
                {
                    textNetwork.IsEnabled = true;
                    textNetwork.Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textPassword, () =>
                {
                    textPassword.IsEnabled = true;
                    textPassword.Invalidate();
                });
            }
            else
            {
                _settings.WriteWifiEnabled(true);
                //_network.Start();

                UXExtensions.DoThreadSafeAction(textWiFi, () =>
                {
                    textWiFi.IsOn = false;
                    textWiFi.Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textNetwork, () =>
                {
                    textNetwork.IsEnabled = false;
                    textNetwork.Invalidate();
                });

                UXExtensions.DoThreadSafeAction(textPassword, () =>
                {
                    textPassword.IsEnabled = false;
                    textPassword.Invalidate();
                });
            }

            Parent.Invalidate();
        }

        private void TextNetwork_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;

            if (text.Length > 0)
            {
                _settings.WriteSsid(text.Trim());
            }
        }

        private void TextPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;

            if (text.Length > 0)
            {
                _settings.WritePassword(text.Trim());
            }
        }
    }
}