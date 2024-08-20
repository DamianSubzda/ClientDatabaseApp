﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientDatabaseApp.Service
{
    public interface IDialogService
    {
        void ShowMessage(string message); 
        bool Confirm(string message);
    }
    internal class DialogService : IDialogService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public bool Confirm(string message)
        {
            var result = MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }
    }
}
