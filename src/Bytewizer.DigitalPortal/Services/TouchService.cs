using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Devices.Gpio;

using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class TouchService
    {
        private readonly SettingsService _settings;

        public FT5xx6Controller Controller { get; private set; }

        public TouchService(SettingsService settings)
        {
            _settings = settings;

            var i2cController = I2cController.FromName(FEZPortal.I2cBus.I2c1);

            var I2cSettings = new I2cConnectionSettings(0x38)
            {
                BusSpeed = 100000,
                AddressFormat = I2cAddressFormat.SevenBit,
            };

            var i2cDevice = i2cController.GetDevice(I2cSettings);

            var gpioController = GpioController.GetDefault();
            var interrupt = gpioController.OpenPin(FEZPortal.GpioPin.TouchInterrupt);

            var orientation = FT5xx6Controller.TouchOrientation.Degrees0;
            if (SettingsService.Flash.FlipOrientation == true)
            {
                orientation = FT5xx6Controller.TouchOrientation.Degrees180;
            }

            Controller = new FT5xx6Controller(i2cDevice, interrupt)
            {
                Width = 480,
                Height = 272,
                Orientation = orientation
            };
        }
    }
}