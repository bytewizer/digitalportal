using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WifiPage : SettingPage
    {
        private TextBox textNetwork;
        private TextBox textPassword;
        private ToggleSwitch textWiFi;

        public WifiPage(int width, int height)
            : base(width, height)
        {
            BackText = ResourcesProvider.UxNavigateBefore;
            Title = "Wireless";

            BackClick += WifiPage_BackClick;

            InitializePage();
        }

        public override void CreateBody()
        {
            SetPageColor(SettingsProvider.Theme.Standard);

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
                ForeColor = SettingsProvider.Theme.Standard,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Left,
                Width = 480 - 40 - 60
            };

            textWiFi = new ToggleSwitch()
            {
                IsOn = SettingsProvider.Flash.NetworkEnabled,
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Highlighted),
                Background = new SolidColorBrush(SettingsProvider.Theme.Shadow),
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
                ForeColor = SettingsProvider.Theme.Standard,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Left,
                Width = 150
            };

            textNetwork = new TextBox()
            {
                Text = SettingsProvider.Flash.Ssid,
                Font = ResourcesProvider.SmallRobotoFont,
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Standard),
                Background = new SolidColorBrush(SettingsProvider.Theme.Shadow),
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
                ForeColor = SettingsProvider.Theme.Standard,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Left,
                Width = 150
            };

            textPassword = new TextBox()
            {
                Text = SettingsProvider.Flash.Password,
                Font = ResourcesProvider.SmallRobotoFont,
                Foreground = new SolidColorBrush(SettingsProvider.Theme.Standard),
                Background = new SolidColorBrush(SettingsProvider.Theme.Shadow),
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
            if (NetworkProvider.IsConnected)
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
                NetworkProvider.Controller.Disable();
                SettingsProvider.WriteWifiEnabled(false);

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
                SettingsProvider.WriteWifiEnabled(true);
                NetworkProvider.EnableWifi();

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
                SettingsProvider.WriteSsid(text.Trim());
            }
        }

        private void TextPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;

            if (text.Length > 0)
            {
                SettingsProvider.WritePassword(text.Trim());
            }
        }
    }
}