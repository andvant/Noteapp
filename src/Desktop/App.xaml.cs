﻿using Noteapp.Core.Exceptions;
using Noteapp.Desktop.ViewModels;
using Noteapp.Desktop.Views;
using System.Windows;
using System.Windows.Threading;

namespace Noteapp.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += HandleGlobalExceptions;

            ApplicationView app = new ApplicationView();
            ApplicationViewModel appViewModel = new ApplicationViewModel();
            app.DataContext = appViewModel;
            app.Show();
        }

        private void HandleGlobalExceptions(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            var ex = args.Exception;
            switch (ex)
            {
                case ApiConnectionException or ApiBadResponseException:
                    MessageBox.Show($"{ex.Message}");
                    args.Handled = true;
                    break;
                default:
                    MessageBox.Show($"{ex.ToString()}");
                    break;
            }
        }
    }
}
