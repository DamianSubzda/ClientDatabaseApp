using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientDatabaseApp.ViewModels
{
    internal interface IBaseDialog
    {
        Action CloseAction { get; set; }
        void ExitWindow(object e);
    }
}
