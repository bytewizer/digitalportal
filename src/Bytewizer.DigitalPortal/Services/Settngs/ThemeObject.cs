using GHIElectronics.TinyCLR.UI.Media;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class ThemeObject
    {
        public ThemeObject()
        {
            Foreground = Colors.White;
            Background = Colors.Black;
        }

        public Color Highlighted { get; internal set; }
        public Color Standard { get; internal set; }
        public Color Shadow { get; internal set; }
        public Color Foreground { get; internal set; }
        public Color Background { get; internal set; }

        public void SetThemeColor(byte[] value)
        {
            var color = Color.FromArgb(value[0], value[1], value[2], value[3]);
            SetThemeColor(color);
        }

        private void SetThemeColor(Color color)
        {
            Highlighted = color;
            Standard = ColorBrightness(color, -.25f);
            Shadow = ColorBrightness(color, -.70f);
        }

        private static Color ColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}