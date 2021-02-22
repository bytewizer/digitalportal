using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public abstract class Page : IWindowPage
    {
        public int Id { get; set; }
        public object State { get; set; } = null;
        public string Text { get; set; }
        public MainWindow Parent { get; set; }
        public UIElement Child { get; set; }
        public bool ShowMenu { get; set; } = false;   
        protected Panel PageBody { get; set; }
        protected int Width { get; set; }
        protected int Height { get; set; }

        protected Page(int width, int height)
            : this(width, height, Orientation.Vertical)
        { }

        protected Page(int width, int height, Orientation orientation)
        {
            Width = width;
            Height = height;

            PageBody = new StackPanel(orientation)
            {
                Width = width,
                Height = height
            };
        }

        public void InitializePage()
        {
            PageBody.Children.Clear();
            PageBody = CreatePageBody();
            Child = PageBody;
        }

        public abstract void OnActivate();

        public abstract Panel CreatePageBody();
    }
}