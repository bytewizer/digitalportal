using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Display;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class DisplayService
    {
        public DisplayController Controller { get; private set; }
        public int Width => Controller.ActiveConfiguration.Width;
        public int Height => Controller.ActiveConfiguration.Height;

        public DisplayService()
        {
            var backlight = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA15);

            backlight.SetDriveMode(GpioPinDriveMode.Output);
            backlight.Write(GpioPinValue.High);

            Controller = DisplayController.GetDefault();

            var orientation = DisplayOrientation.Degrees0;
            if (SettingsService.Flash.FlipOrientation == true)
            {
                orientation = DisplayOrientation.Degrees180;
            }
            Controller.SetConfiguration(new ParallelDisplayControllerSettings
            {
                Width = 480,
                Height = 272,
                DataFormat = DisplayDataFormat.Rgb565,
                Orientation = orientation,
                PixelClockRate = 10000000,
                PixelPolarity = false,
                DataEnablePolarity = false,
                DataEnableIsFixed = false,
                HorizontalFrontPorch = 2,
                HorizontalBackPorch = 2,
                HorizontalSyncPulseWidth = 41,
                HorizontalSyncPolarity = false,
                VerticalFrontPorch = 2,
                VerticalBackPorch = 2,
                VerticalSyncPulseWidth = 10,
                VerticalSyncPolarity = false,
            });

            Controller.Enable();
        }
    }
}