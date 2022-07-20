using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Controls;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class DigitalText : TextBox
    {
        public DigitalText()
        {
            Font = ResourcesProvider.SmallDigitalFont;
            Foreground = new SolidColorBrush(SettingsService.Theme.Standard);
            Background = new SolidColorBrush(SettingsService.Theme.Background);
            IsEnabled = false;
        }
    }
}