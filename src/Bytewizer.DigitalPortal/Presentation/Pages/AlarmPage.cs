using System;
using System.Collections;

using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Input;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Threading;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class AlarmPage : Page
    {
        public AlarmPage(DisplayService display)
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
                Text = "this feature is not available",
                TextAlign = TextAlignment.Center,
                Width = Width
            };

            PageBody.Children.Add(textNotAvailable);

            return PageBody;
        }
    }
}