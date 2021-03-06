using System.Windows;
using System.Windows.Controls;

namespace Noteapp.Desktop.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password = ((PasswordBox)sender).Password;
            ((dynamic)this.DataContext).Password = password;

            placeholderPassword.Visibility = string.IsNullOrEmpty(password) ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private void EmailChanged(object sender, TextChangedEventArgs e)
        {
            var email = ((TextBox)sender).Text;
            placeholderEmail.Visibility = string.IsNullOrEmpty(email) ?
                Visibility.Visible : Visibility.Collapsed;
        }
    }
}
