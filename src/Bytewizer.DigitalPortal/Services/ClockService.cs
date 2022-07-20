using System;

using GHIElectronics.TinyCLR.Devices.Rtc;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class ClockService
    {
        public RtcController Controller { get; private set; }

        private int _timezoneOffset;

        public ClockService(SettingsService settings)
        {
            Controller = RtcController.GetDefault();
            Controller.SetChargeMode(BatteryChargeMode.Fast);

            _timezoneOffset = settings.GetTimeOffset();
        }

        public DateTime UTC { get => Controller.Now; }
        
        public DateTime LocalTime 
        { 
            get { return Controller.Now.AddSeconds(_timezoneOffset); }
        }

        public void SetTime(DateTime value, int seconds)
        {
            Controller.SetTime(RtcDateTime.FromDateTime(value));
            _timezoneOffset = seconds;
        }
    }
}