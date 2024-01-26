using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class MVMainWindow
    {
        public ICommand TabSelectionChangedCommand { get; private set; }
        public MVMainWindow()
        {
            TabSelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(TabSelectionChanged);
        }


        private void TabSelectionChanged(SelectionChangedEventArgs e)
        {
            //if (e.AddedItems[0] is TabItem selectedTabItem)
            //{
            //    if (selectedTabItem.Header.ToString() == "Baza klientów")
            //    {
            //        //InitializeClientsGrid();
            //    }
            //}
        }
    }
}
