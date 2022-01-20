using Noteapp.Desktop.Data;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Security;
using System;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class LoginViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Login;

        private readonly ApiService _apiService;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool StaySignedIn { get; set; } = true;

        private string _outputMessage;
        public string OutputMessage
        {
            get => _outputMessage;
            set => Set(ref _outputMessage, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiService apiService)
        {
            ArgumentNullException.ThrowIfNull(apiService);

            _apiService = apiService;
            LoginCommand = new RelayCommand(Login);
        }

        private async void Login()
        {
            OutputMessage = string.Empty;
            var result = await _apiService.Login(Email, Password);
            if (result.IsSuccess)
            {
                AppData.DeleteLocalData();
                var userInfoResponse = result.UserInfoResponse;
                var encryptionKey = Protector.GenerateEncryptionKey(Password, userInfoResponse.EncryptionSalt);
                await AppData.CreateAndSaveUserInfo(userInfoResponse, encryptionKey, StaySignedIn);
                OutputMessage = "Successfully logged in";
            }
            else
            {
                OutputMessage = $"Failed to log in: {result.ErrorMessage}";
            }
        }

        public void RefreshPage()
        {
            OutputMessage = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }
    }
}

