using System;
using System.Reflection;

using Bytewizer.TinyCLR.DigitalPortal.Client.Models;

using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.Devices.SecureStorage;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class SettingsService
    {
        public SecureStorageController Controller { get; private set; }
        
        public static FlashObject Flash { get; private set; }
        public static ThemeObject Theme { get; private set; }
        public static WeatherObject Weather { get; private set; }
        public static ResourceObject Resource { get => new ResourceObject(); }
        
        public static bool NetworkConnected { get; private set; } = false;

        public SettingsService()
        {
            Controller = new SecureStorageController(SecureStorage.Configuration);

            Flash = ReadFlash();

            Theme = new ThemeObject();
            Theme.SetThemeColor(Flash.ThemeColor);

            Weather = new WeatherObject();
        }

        public int GetTimeOffset()
        {
            return Flash.TimeOffset;
        }

        public void SetWeather(WeatherObject value)
        {
            Weather = value;
        }

        public void SetNetworkConnected(bool value)
        {
            NetworkConnected = value;
        }

        public void WriteTheme(Color color)
        {
            var value = new byte[] { color.A, color.R, color.G, color.B };
            Theme.SetThemeColor(value);
            Flash.ThemeColor = value;
            Write();
        }

        public void WriteWifiEnabled(bool value)
        {
            Flash.NetworkEnabled = value;
            Write();
        }

        public void WriteSsid(string value)
        {
            Flash.Ssid = value;
            Write();
        }

        public void WritePassword(string value)
        {
            Flash.Password = value;
            Write();
        }

        public void WriteUnits(Units value)
        {
            Flash.Units = value;
            Write();
        }

        public void WriteTimeOffset(int value)
        {
            Flash.TimeOffset = value;
            Write();
        }

        public void WriteOrientation(bool value)
        {
            Flash.FlipOrientation = value;
            Write();
        }

        public void WriteLocation(string value)
        {
            if (Flash.Location != value)
            {
                Flash.Location = value;
                Write();
            }
        }

        public void WriteLat(string value)
        {
            if (Flash.Lat != value)
            {
                Flash.Lat = value;
                Write();
            }
        }

        public void WriteLon(string value)
        {
            if (Flash.Lon != value)
            {
                Flash.Lon = value;
                Write();
            }
        }

        public void WriteQuery(string value)
        {
            Flash.Query = value;
            Write();
        }

        public void WriteShowDow(bool value)
        {
            Flash.ShowDow = value;
            Write();
        }

        public bool IsErased()
        {
            var isBlank = true;

            for (uint block = 0; block < Controller.TotalSize / Controller.BlockSize; block++)
            {
                if (!Controller.IsBlank(block)) isBlank = false;
            }

            return isBlank;
        }

        public void WriteFlash(FlashObject settings)
        {

            var buffer = new byte[1 * 1024];
            byte[] flashBuffer = Reflection.Serialize(settings, typeof(FlashObject));

            Array.Copy(BitConverter.GetBytes(flashBuffer.Length), buffer, 4);
            Array.Copy(flashBuffer, 0, buffer, 4, flashBuffer.Length);

            if (IsErased() == false)
            {
                Controller.Erase();
            }

            var dataBlock = new byte[Controller.BlockSize];

            for (uint block = 0; block < buffer.Length / Controller.BlockSize; block++)
            {
                Array.Copy(buffer, (int)(block * Controller.BlockSize), dataBlock, 0, (int)(Controller.BlockSize));
                Controller.Write(block, dataBlock);
            }
        }

        private FlashObject ReadFlash()
        {
            if (IsErased() == false)
            {
                var buffer = new byte[1 * 1024];
                var dataBlock = new byte[Controller.BlockSize];

                for (uint block = 0; block < buffer.Length / Controller.BlockSize; block++)
                {
                    Controller.Read(block, dataBlock);
                    Array.Copy(dataBlock, 0, buffer, (int)(block * Controller.BlockSize), dataBlock.Length);
                }

                var length = BitConverter.ToInt16(buffer, 0);

                byte[] flashBuffer = new byte[length];
                Array.Copy(buffer, 4, flashBuffer, 0, length);

                return (FlashObject)Reflection.Deserialize(flashBuffer, typeof(FlashObject));
            }
            else
            {
                WriteFlash(new FlashObject());
                return ReadFlash();
            }

        }

        private void Write()
        {
            WriteFlash(Flash);
        }
    }
}