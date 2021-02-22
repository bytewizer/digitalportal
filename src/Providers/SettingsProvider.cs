using System;
using System.Reflection;

using Bytewizer.TinyCLR.DigitalPortal.Client.Models;

using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.Devices.SecureStorage;

namespace Bytewizer.TinyCLR.DigitalPortal
{   
    public class SettingsProvider
    {
        private static bool _initialized;
        private static readonly object _lock = new object();

        public static SecureStorageController Controller { get; private set; }
        public static FlashObject Flash { get; private set; }
        public static ThemeObject Theme { get; private set; }

        public static void Initialize()
        {
            if (_initialized)
                return;

            Controller = new SecureStorageController(SecureStorage.Configuration);

            Flash = ReadFlash();

            Theme = new ThemeObject();
            Theme.SetThemeColor(Flash.ThemeColor);

            _initialized = true;
        }

        public static void WriteTheme(Color color)
        {
            var value = new byte[] { color.A, color.R, color.G, color.B };
            Theme.SetThemeColor(value);
            Flash.ThemeColor = value;
            Write();
        }

        public static void WriteWifiEnabled(bool value)
        {
            Flash.NetworkEnabled = value;
            Write();
        }

        public static void WriteSsid(string value)
        {
            Flash.Ssid = value;
            Write();
        }

        public static void WritePassword(string value)
        {
            Flash.Password = value;
            Write();
        }

        public static void WriteUnits(Units value)
        {
            Flash.Units = value;
            Write();
        }

        public static void WriteOrientation(bool value)
        {
            Flash.FlipOrientation = value;
            Write();
        }

        public static void WriteLocation(string value)
        {
            if (Flash.Location != value)
            {
                Flash.Location = value;
                Write();
            }
        }

        public static void WriteLat(string value)
        {
            if (Flash.Lat != value)
            {
                Flash.Lat = value;
                Write();
            }
        }

        public static void WriteLon(string value)
        {
            if (Flash.Lon != value)
            {
                Flash.Lon = value;
                Write();
            }
        }

        public static void WriteQuery(string value)
        {
            Flash.Query = value;
            Write();
        }

        public static void WriteShowDow(bool value)
        {
            Flash.ShowDow = value;
            Write();
        }

        public static bool IsErased()
        {
            var isBlank = true;

            for (uint block = 0; block < Controller.TotalSize / Controller.BlockSize; block++)
            {
                if (!Controller.IsBlank(block)) isBlank = false;
            }

            return isBlank;
        }

        public static void WriteFlash(FlashObject settings)
        {
            lock (_lock)
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
        }

        private static FlashObject ReadFlash()
        {
            lock (_lock)
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
        }

        private static void Write()
        {
            WriteFlash(Flash);
        }
    }
}