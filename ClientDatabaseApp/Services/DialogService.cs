using ClientDatabaseApp.ViewModels;
using Microsoft.Win32;
using System;
using System.Windows;

namespace ClientDatabaseApp.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message);
        bool Confirm(string message);
        void ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class;
        string OpenFileDialog(string filter, string title);
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

        public void ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var viewModelType = typeof(TViewModel);
            var viewName = viewModelType.Name.Replace("ViewModel", "");
            var viewType = Type.GetType($"ClientDatabaseApp.Views.{viewName}");

            if (viewType == null)
            {
                throw new InvalidOperationException($"View type '{viewName}' not found.");
            }

            var view = Activator.CreateInstance(viewType) as Window;
            if (view != null)
            {
                view.DataContext = viewModel;

                if (viewModel is IBaseDialog baseDialog)
                {
                    baseDialog.CloseAction = () => view.Close();
                }

                view.ShowDialog();
            }
            else
            {
                throw new InvalidOperationException($"Unable to create instance of view type '{viewName}'.");
            }
        }

        public string OpenFileDialog(string filter, string title)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = filter,
                Title = title
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }
    }
}
