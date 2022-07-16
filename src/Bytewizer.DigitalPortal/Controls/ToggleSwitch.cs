using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Input;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class ToggleSwitch : Control
    {
        public event RoutedEventHandler Click;
        public event RoutedEventHandler On;

        public bool IsOn { get; set; }

        public ToggleSwitch()
        {

        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            var evt = new RoutedEvent("TouchUpEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler));
            var args = new RoutedEventArgs(evt, this);

            this.Click?.Invoke(this, args);

            e.Handled = args.Handled;

            this.IsOn = !this.IsOn;

            evt = new RoutedEvent(this.IsOn ? "CheckedEvent" : "UncheckedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler));
            args = new RoutedEventArgs(evt, this);
            if (this.IsOn)
            {
                this.On?.Invoke(this, args);
            }
            //else
            //{
            //    this.On?.Invoke(this, args);
            //}

            if (this.Parent != null)
                this.Invalidate();
        }

        public override void OnRender(DrawingContext dc)
        {
            //var x = 0; // width
            //var y = 0; // height

            //x += (Width / 4) - 1;
            //y += (Height / 4) - 1;

            var brush2 = new SolidColorBrush(Colors.White);

            if (IsOn == true)
            {         
                dc.DrawRectangle(_foreground, null, 0, Height - 30, 60, 30);
                dc.DrawRectangle(brush2, null, 30 + 3, Height - 30 + 3, 30 - 6, 30 - 6); // isOn
            }
            else
            {
                dc.DrawRectangle(_background, null, 0, Height - 30, 60, 30);
                dc.DrawRectangle(brush2, null, 3, Height - 30 + 3, 30 - 6, 30 - 6); // isOff
            }
        }
    }
}