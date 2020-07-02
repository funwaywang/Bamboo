using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace System.Windows
{
    public static class UIElementExtensions
    {
        public static void ShowMessageBox(this UIElement uIElement, Exception ex)
        {
            MessageBox.Show(Window.GetWindow(uIElement), ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowMessageBox(this UIElement uIElement, string message, MessageBoxImage icon)
        {
            MessageBox.Show(Window.GetWindow(uIElement), message, "Exception", MessageBoxButton.OK, icon);
        }
    }
}
