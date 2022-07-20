using System;

using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class ApplicationService : Application
    {              
        public ApplicationService(DisplayService display, TouchService touch)
        : base(display.Controller) 
        {
            touch.Controller.TouchDown += Touch_TouchDown;
            touch.Controller.TouchUp += Touch_TouchUp;
        }

        private void Touch_TouchUp(FT5xx6Controller sender, TouchEventArgs e) =>
            this.InputProvider.RaiseTouch(e.X, e.Y, GHIElectronics.TinyCLR.UI.Input.TouchMessages.Up, DateTime.Now);

        private void Touch_TouchDown(FT5xx6Controller sender, TouchEventArgs e) =>
            this.InputProvider.RaiseTouch(e.X, e.Y, GHIElectronics.TinyCLR.UI.Input.TouchMessages.Down, DateTime.Now);
    }
}