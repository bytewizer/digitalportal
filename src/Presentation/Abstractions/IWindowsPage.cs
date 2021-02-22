using GHIElectronics.TinyCLR.UI;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public interface IWindowPage
    {
        int Id { get; set; }
        object State { get; set; }
        MainWindow Parent { get; set; }
        UIElement Child { get; set; }
        bool ShowMenu { get; set; }
        void InitializePage();
        void OnActivate();
    }
}
