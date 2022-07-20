using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Input;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Shapes;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class ThemePage : SettingPage
    {
        private SettingsService _settings;

        private Border[] borders;
        private Rectangle[] rectangles;

        private Color colorSelected;

        public ThemePage(DisplayService display, SettingsService settings)
            : base(display.Width, display.Height)
        {
            _settings = settings;

            BackText = ResourcesProvider.UxNavigateBefore;
            Title = "theme color";

            BackClick += ThemePage_BackClick;

            InitializePage();
        }

        public override void CreateBody()
        {
            colorSelected = SettingsService.Theme.Highlighted;

            var panelPallet = new StackPanel(Orientation.Vertical)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            panelPallet.SetMargin(10);

            var colors = new Color[9]
            {
                ThemeColors.Pink,
                ThemeColors.Red,
                ThemeColors.Orange,
                ThemeColors.Tan,
                ThemeColors.Blue,
                ThemeColors.Lagoon,
                ThemeColors.Green,
                ThemeColors.Yellow,
                ThemeColors.Purple
            };

            var i = 0;
            var panelRow = new StackPanel[3];

            borders = new Border[9];
            rectangles = new Rectangle[9];
            
            for (var x = 0; x < 3; x++)
            {
                panelRow[x] = new StackPanel(Orientation.Horizontal)
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                for (var y = 0; y < 3; y++)
                {
                    rectangles[i] = new Rectangle((Width - 35) / 3, (Height - 75) / 3)
                    {
                        Fill = new SolidColorBrush(colors[i]),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    rectangles[i].TouchUp += RectangleColor_TouchUp;

                    borders[i] = new Border
                    {
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        Child = rectangles[i]
                    };
                    borders[i].SetBorderThickness(1);
                    borders[i].SetMargin(2);

                    panelRow[x].Children.Add(borders[i]);
                    panelRow[x].Height = ((Height - 40) / 3) - 20;

                    i++;
                }
                panelPallet.Children.Add(panelRow[x]);
            }

            PageBody.Children.Add(panelPallet);
        }

        public override void OnActivate()
        {
            var fill = new SolidColorBrush(SettingsService.Theme.Highlighted);

            for (var i = 0; i < rectangles.Length; i++)
            {
                if (((SolidColorBrush)rectangles[i].Fill) == fill)
                {
                    UXExtensions.DoThreadSafeAction(borders[i], () =>
                    {
                        borders[i].BorderBrush = new SolidColorBrush(Colors.White);
                        borders[i].Invalidate();
                    });
                }
                else
                {
                    UXExtensions.DoThreadSafeAction(borders[i], () =>
                    {
                        borders[i].BorderBrush = new SolidColorBrush(Colors.Black);
                        borders[i].Invalidate();
                    });
                }
            }
        }

        private void ThemePage_BackClick(object sender, RoutedEventArgs e)
        {
            _settings.WriteTheme(colorSelected);

            Parent.Refresh();
            Parent.Activate(3);
        }

        private void RectangleColor_TouchUp(object sender, TouchEventArgs e)
        {
            var fill = (SolidColorBrush)((Rectangle)e.Device.Target).Fill;
            colorSelected = fill.Color;
            SetPageColor(colorSelected);

            for (var i = 0; i < rectangles.Length; i++)
            {
                UXExtensions.DoThreadSafeAction(borders[i], () =>
                {
                    if (rectangles[i].Fill == fill)
                    {
                        borders[i].BorderBrush = new SolidColorBrush(Colors.White);
                        borders[i].Invalidate();
                    }
                    else
                    {
                        borders[i].BorderBrush = new SolidColorBrush(Colors.Black);
                        borders[i].Invalidate();
                    }
                });
            }

            Parent.Invalidate();
        }
    }
}