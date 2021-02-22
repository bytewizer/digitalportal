using System;

using GHIElectronics.TinyCLR.UI;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public static class UXExtensions
    {
        public static void DoThreadSafeAction(UIElement uiElem, Action action)
        {
            if (uiElem.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(1), _ =>
                {
                    action();
                    return null;
                }, null);
            }
        }
    }
}