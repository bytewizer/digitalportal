using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class NotAvailablePage : Page
    {
        public NotAvailablePage(DisplayService display)
            : base(display.Width, display.Height, Orientation.Horizontal)
        {
            ShowMenu = true;
            InitializePage();
        }

        public override void OnActivate() { }

        public override Panel CreatePageBody()
        {
            var textNotAvailable = new DigitalText()
            {
                Text = "internet access is not available",
                TextAlign = TextAlignment.Center,
                Width = Width
            };

            PageBody.Children.Add(textNotAvailable);

            return PageBody;
        }
    }
}