using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class DigitalText : TextBox
    {
        public DigitalText()
        {
            Font = ResourcesProvider.SmallDigitalFont;
            Foreground = new SolidColorBrush(SettingsProvider.Theme.Standard);
            Background = new SolidColorBrush(SettingsProvider.Theme.Background);
            IsEnabled = false;
        }
    }
}