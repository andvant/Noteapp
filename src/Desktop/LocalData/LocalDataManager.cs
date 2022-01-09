using Microsoft.Win32;
using Noteapp.Desktop.Dtos;
using Noteapp.Desktop.Extensions;
using Noteapp.Desktop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Noteapp.Desktop.LocalData
{
    public static class LocalDataManager
    {
        private static readonly string _userInfoPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "noteapp_userinfo.json");

        private static readonly string _notesPath = "notes.json";

        public static async Task SaveUserInfoResponse(UserInfoResponse userInfoDto, string encryptionKey)
        {
            var userInfo = new UserInfo()
            {
                Email = userInfoDto.email,
                AccessToken = userInfoDto.access_token,
                RegistrationDate = DateTime.Parse(userInfoDto.registration_date),
                EncryptionKey = encryptionKey,
                EncryptionEnabled = true
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
            return Application.Current.Properties["userInfo"] as UserInfo ?? new UserInfo();
        }

        private static UserInfo GetUserInfoFromFile()
        {
            try
            {
                return File.ReadAllText(_userInfoPath).FromJson<UserInfo>();
            }
            catch
            {
                return null;
            }
        }

        public static void LoadUserInfoToMemory()
        {
            Application.Current.Properties["userInfo"] = GetUserInfoFromFile();
        }

        public static void DeleteUserInfo()
        {
            Application.Current.Properties["userInfo"] = null;

            if (File.Exists(_userInfoPath))
            {
                File.Delete(_userInfoPath);
            }
        }

        public static IEnumerable<Note> GetNotes()
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

        public static async Task SaveNotes(IEnumerable<Note> notes)
        {
            await File.WriteAllTextAsync(_notesPath, notes.ToJson());
        }

        public static void DeleteNotes()
        {
            if (File.Exists(_notesPath))
            {
                File.Delete(_notesPath);
            }
        }

        public static async Task ExportNotes(IEnumerable<Note> notes)
        {
            var saveDialog = new SaveFileDialog()
            {
                FileName = $"ExportedNotes-{DateTime.Now.ToShortDateString()}",
                Filter = "JSON file|*.json"
            };
            var success = saveDialog.ShowDialog();
            if (success == true)
            {
                await File.WriteAllTextAsync(saveDialog.FileName, notes.ToJson());
            }
        }

        public static async Task<IEnumerable<Note>> ImportNotes()
        {
            var openDialog = new OpenFileDialog()
            {
                Filter = "JSON file|*.json"
            };
            var success = openDialog.ShowDialog();
            if (success == false) return null;

            string notesJson = await File.ReadAllTextAsync(openDialog.FileName);
            return notesJson.FromJson<IEnumerable<Note>>();
        }
    }
}
