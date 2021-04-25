using System;
using System.Drawing;
using System.Threading;

using Bytewizer.MatrixRain.Properties;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Display;

namespace Bytewizer.MatrixRain
{
    class Program
    {
        static readonly Font digitalFont
            = Resources.GetFont(Resources.FontResources.RobotoFont);

        static readonly Random rand = new Random();

        static char AsciiCharacter
        {
            get
            {
                int t = rand.Next(10);
                if (t <= 2)
                    return (char)('0' + rand.Next(10));
                else if (t <= 4)
                    return (char)('a' + rand.Next(27));
                else if (t <= 6)
                    return (char)('A' + rand.Next(27));
                else
                    return (char)(rand.Next(255));
            }
        }

        static void Main()
        {
            var backlight = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA15);
            backlight.SetDriveMode(GpioPinDriveMode.Output);
            backlight.Write(GpioPinValue.High);

            var controller = DisplayController.GetDefault();
            controller.SetConfiguration(new ParallelDisplayControllerSettings
            {
                Width = 480,
                Height = 272,
                DataFormat = DisplayDataFormat.Rgb565,
                Orientation = DisplayOrientation.Degrees180,
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

            var display = Graphics.FromHdc(controller.Hdc);

            var fontHeight = 16;
            var fontWidth = 16;
            var height = controller.ActiveConfiguration.Height / fontHeight;
            var width = controller.ActiveConfiguration.Width / fontWidth;
            var y = new int[width];

            for (int i = 0; i < width; ++i)
            {
                y[i] = rand.Next(height);
            }

            controller.Enable();
            display.Clear();

            var black = new SolidBrush(Color.Black);
            var white = new SolidBrush(Color.White);
            var green = new SolidBrush(Color.Green);

            while (true)
            {
                int x;
                for (x = 0; x < width; ++x)
                {
                    display.FillRectangle(black, x * fontWidth, y[x] * fontHeight, fontWidth, fontHeight);
                    display.DrawString(AsciiCharacter.ToString(), digitalFont, white, x * fontWidth, y[x] * fontHeight);

                    int temp = y[x] - 1;
                    display.FillRectangle(black, x * fontWidth, InScreenYPosition(temp, height) * fontHeight, fontWidth, fontHeight);
                    display.DrawString(AsciiCharacter.ToString(), digitalFont, green, x * fontWidth, InScreenYPosition(temp, height) * fontHeight);

                    int temp1 = y[x] - 10;
                    display.FillRectangle(black, x * fontWidth, InScreenYPosition(temp1, height) * fontHeight, fontWidth, fontHeight);

                    y[x] = InScreenYPosition(y[x] + 1, height);
                }
                display.Flush();
                
                Thread.Sleep(75);
            }
        }

        public static int InScreenYPosition(int yPosition, int height)
        {
            if (yPosition < 0)
                return yPosition + height;
            else if (yPosition < height)
                return yPosition;
            else
                return 0;
        }
    }
}