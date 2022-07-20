using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Input;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Shapes;
using GHIElectronics.TinyCLR.UI.Controls;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public abstract class SettingPage : Page
    {
        public const int TitleHeight = 40;
        public const int ButtonWidth = 50;

        public string Title { get; set; }
        public string BackText { get; set; }
        public string NextText { get; set; }

        public event RoutedEventHandler BackClick;
        public event RoutedEventHandler NextClick;

        public Text buttonBack;
        public Text buttonNext;
        public Text textTitle;
        public Line lineTitle;

        protected SettingPage(int width, int height)
            : base(width, height)
        {
        }

        public override Panel CreatePageBody()
        {
            CreateTitleBar();
            CreateBody();

            SetPageColor(SettingsService.Theme.Highlighted);

            return PageBody;
        }

        public void CreateTitleBar()
        {
            var panelTitle = new StackPanel(Orientation.Horizontal)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = TitleHeight
            };

            buttonBack = new Text()
            {
                TextContent = BackText,
                Font = ResourcesProvider.SmallUxIcons,
                ForeColor = Colors.White,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = ButtonWidth
            };
            buttonBack.TouchUp += ButtonBack_TouchUp;
            panelTitle.Children.Add(buttonBack);

            textTitle = new Text()
            {
                TextContent = Title,
                Font = ResourcesProvider.SmallDigitalFont,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = Width - (ButtonWidth * 2)
            };
            panelTitle.Children.Add(textTitle);

            buttonNext = new Text()
            {
                TextContent = NextText,
                Font = ResourcesProvider.SmallUxIcons,
                ForeColor = Colors.White,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = ButtonWidth
            };
            buttonNext.TouchUp += ButtonNext_TouchUp;
            panelTitle.Children.Add(buttonNext);

            lineTitle = new Line(Width - 20, 0);

            var panelLine = new StackPanel(Orientation.Vertical)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = Width
            };
            panelLine.Children.Add(lineTitle);

            PageBody.Children.Add(panelTitle);
            PageBody.Children.Add(panelLine);
        }

        public abstract void CreateBody();

        public void SetPageColor(Color color)
        {
            textTitle.ForeColor = color;
            lineTitle.Stroke = new Pen(color);
        }


        private void ButtonBack_TouchUp(object sender, TouchEventArgs e)
        {
            var evt = new RoutedEvent("TouchUpEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler));
            var args = new RoutedEventArgs(evt, this);

            BackClick?.Invoke(this, args);

            e.Handled = args.Handled;
        }

        private void ButtonNext_TouchUp(object sender, TouchEventArgs e)
        {
            var evt = new RoutedEvent("TouchUpEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler));
            var args = new RoutedEventArgs(evt, this);

            NextClick?.Invoke(this, args);

            e.Handled = args.Handled;
        }
    }
}