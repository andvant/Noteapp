using Noteapp.Desktop.Extensions;
using Noteapp.Desktop.Models;
using System;
using System.Collections.Generic;
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

        private static readonly string _notesPath = "notes.json";

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

        private static UserInfo GetUserInfoFromFile()
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

        public static void LoadUserInfoToMemory()
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

        public static IEnumerable<Note> GetLocalNotes()
        {
            try
            {
                return File.ReadAllText(_notesPath).FromJson<IEnumerable<Note>>();
            }
            catch
            {
                return new List<Note>();
            }
        }

        public static void SaveLocalNotes(IEnumerable<Note> notes)
        {
            File.WriteAllText(_notesPath, notes.ToJson());
        }

        public static void DeleteLocalNotes()
        {
            if (File.Exists(_notesPath))
            {
                File.Delete(_notesPath);
            }
        }
    }
}
