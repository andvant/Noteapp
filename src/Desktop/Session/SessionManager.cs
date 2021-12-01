using Noteapp.Desktop.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Noteapp.Desktop.Session
{
    public static class SessionManager
    {
        private static readonly string _userInfoPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "noteapp_userinfo.json");

        public static async Task SaveUserInfo(UserInfoDto userInfoDto, string encryptionKey)
        {
            var userInfo = new UserInfo()
            {
                AccessToken = userInfoDto.access_token,
                Email = userInfoDto.email,
                EncryptionKey = encryptionKey
            };

            await SaveUserInfo(userInfo);
        }

        public static async Task SaveUserInfo(UserInfo userInfo)
        {
            Application.Current.Properties["userInfo"] = userInfo;
            await File.WriteAllTextAsync(_userInfoPath, userInfo.ToJson());
        }

        public static UserInfo GetUserInfo()
        {
            return Application.Current.Properties["userInfo"] as UserInfo;
        }

        public static UserInfo GetUserInfoFromFile()
        {
            try
            {
                var userInfoJson = File.ReadAllText(_userInfoPath);
                return userInfoJson.FromJson<UserInfo>();
            }
            catch
            {
                return null;
            }
        }

        public static void LoadUserInfo()
        {
            var userInfo = GetUserInfoFromFile();
            Application.Current.Properties["userInfo"] = userInfo;
        }

        public static void DeleteUserInfo()
        {
            Application.Current.Properties["userInfo"] = null;

            if (File.Exists(_userInfoPath))
            {
                File.Delete(_userInfoPath);
            }
        }
    }
}
