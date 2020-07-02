using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bamboo
{
    public static class Commands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(nameof(Exit), nameof(Exit), typeof(Commands));
    }
}
