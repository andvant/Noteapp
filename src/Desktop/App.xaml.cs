﻿using Noteapp.Desktop.Exceptions;
using Noteapp.Desktop.Security;
using Noteapp.Desktop.ViewModels;
using Noteapp.Desktop.Views;
using System.Diagnostics;
using System.Security.Cryptography;
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
                case CryptographicException:
                    MessageBox.Show($"Encryption/decryption failed: invalid encryption key.");
                    args.Handled = true;
                    break;
                default:
                    MessageBox.Show($"UNEXPECTED EXCEPTION:\n{ex.ToString()}\n\nThe application will be closed.");
                    break;
            }
        }
    }
}
