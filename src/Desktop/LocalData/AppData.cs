using Microsoft.Win32;
using Noteapp.Desktop.Dtos;
using Noteapp.Desktop.Extensions;
using Noteapp.Desktop.LocalData;
using Noteapp.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Noteapp.Desktop.Data
{
    public static class AppData
    {
        private static readonly string _userInfoPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "noteapp_userinfo.json");

        private static readonly string _notesPath = "notes.json";

        private static bool _isPersisted = true;

        public static ObservableCollection<Note> Notes { get; set; }
        public static UserInfo UserInfo { get; set; }

        public static async Task CreateAndSaveUserInfo(UserInfoResponse userInfoDto, string encryptionKey, bool isPersisted)
        {
            var userInfo = new UserInfo()
            {
                Email = userInfoDto.Email,
                AccessToken = userInfoDto.AccessToken,
                RegistrationDate = userInfoDto.RegistrationDate,
                EncryptionKey = encryptionKey,
                EncryptionEnabled = true
            };

            _isPersisted = isPersisted;
            UserInfo = userInfo;
            await SaveUserInfo();
        }

        public static async Task SaveUserInfo()
        {
            if (_isPersisted)
            {
                await File.WriteAllTextAsync(_userInfoPath, UserInfo.ToJson());
            }
        }

        public static void LoadUserInfoToMemory()
        {
            UserInfo = ReadUserInfoFromFile();
        }

        private static UserInfo ReadUserInfoFromFile()
        {
            try
            {
                return File.ReadAllText(_userInfoPath).FromJson<UserInfo>();
            }
            catch
            {
                return new UserInfo();
            }
        }

        public static IEnumerable<Note> ReadNotes()
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

        public static async Task SaveNotes()
        {
            if (_isPersisted)
            {
                await File.WriteAllTextAsync(_notesPath, Notes.ToJson());
            }
        }

        public static async Task ExportNotes()
        {
            var saveDialog = new SaveFileDialog()
            {
                FileName = $"ExportedNotes-{DateTime.Now.ToShortDateString()}",
                Filter = "JSON file|*.json"
            };
            var success = saveDialog.ShowDialog();
            if (success == true)
            {
                await File.WriteAllTextAsync(saveDialog.FileName, Notes.ToJson());
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

        public static void DeleteLocalData()
        {
            UserInfo = new UserInfo();
            Notes.Clear();
            if (File.Exists(_userInfoPath)) File.Delete(_userInfoPath);
            if (File.Exists(_notesPath)) File.Delete(_notesPath);
        }
    }
}
