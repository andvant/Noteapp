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
                EncryptionKey = encryptionKey,
                EncryptionSalt = userInfoDto.encryption_salt
            };

            Application.Current.Properties["userInfo"] = userInfo;

            var userInfoJson = userInfo.ToJson();
            await File.WriteAllTextAsync(_userInfoPath, userInfoJson);
        }

        public static async Task<UserInfo> GetUserInfo()
        {
            var userInfo = Application.Current.Properties["userInfo"] as UserInfo;
            if (userInfo != null) return userInfo;
            if (!File.Exists(_userInfoPath)) return null;

            var userInfoJson = await File.ReadAllTextAsync(_userInfoPath);
            return userInfoJson.FromJson<UserInfo>();
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
