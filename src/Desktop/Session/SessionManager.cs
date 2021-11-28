using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Noteapp.Desktop.Session
{
    // TODO: use configuration
    public class SessionManager
    {
        private readonly string _userInfoPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "noteapp_userinfo.json");

        public async Task SaveUserInfo(UserInfoDto userInfoDto, string encryptionKey)
        {
            var userInfo = new UserInfo()
            {
                AccessToken = userInfoDto.access_token,
                Email = userInfoDto.email,
                EncryptionKey = encryptionKey,
                EncryptionSalt = userInfoDto.encryption_salt
            };

            var userInfoJson = JsonSerializer.Serialize(userInfo, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_userInfoPath, userInfoJson);
        }

        public async Task<UserInfo> GetUserInfo()
        {
            if (!File.Exists(_userInfoPath)) return null;
            var userInfoJson = await File.ReadAllTextAsync(_userInfoPath);
            return JsonSerializer.Deserialize<UserInfo>(userInfoJson);
        }

        public void DeleteUserInfo()
        {
            if (File.Exists(_userInfoPath))
            {
                File.Delete(_userInfoPath);
            }
        }
    }
}
