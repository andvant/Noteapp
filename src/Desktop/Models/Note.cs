using Noteapp.Desktop.MVVM;
using System;
using System.Text.Json.Serialization;

namespace Noteapp.Desktop.Models
{
    public class Note : NotifyPropertyChanged
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                Set(ref _text, value);
                TextChanged = true;
                OnPropertyChanged(nameof(TextPreview));
            }
        }

        public string TextPreview
        {
            get
            {
                var preview = Text?.Split('\n', '\r')?[0];
                preview = preview?.Substring(0, Math.Min(preview.Length, 30));
                return string.IsNullOrWhiteSpace(preview) ? "New note..." : preview;
            }
        }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        private DateTime? _updatedLocal;
        public DateTime UpdatedLocal
        {
            get => _updatedLocal ?? Updated;
            set => Set(ref _updatedLocal, value);
        }

        private bool _locked;
        public bool Locked
        {
            get => _locked;
            set => Set(ref _locked, value);
        }
        private bool _archived;
        public bool Archived
        {
            get => _archived;
            set => Set(ref _archived, value);
        }
        private bool _pinned;
        public bool Pinned
        {
            get => _pinned;
            set => Set(ref _pinned, value);
        }
        private string _publicUrl;
        public string PublicUrl
        {
            get => _publicUrl;
            set
            {
                Set(ref _publicUrl, value);
                OnPropertyChanged(nameof(Published));
            }
        }

        private bool _published;
        public bool Published
        {
            get => _published;
            set => Set(ref _published, value);
        }
        [JsonIgnore]
        public bool TextChanged { get; set; } = false;
        public bool Synchronized { get; set; } = true;
        public bool Local { get; set; } = false;
        public bool Deleted { get; set; } = false;

        public static Note CreateLocalNote(bool archived)
        {
            return new Note()
            {
                Id = 0,
                Synchronized = false,
                Local = true,
                Archived = archived,
                UpdatedLocal = DateTime.Now,
                Text = string.Empty
            };
        }

        public static bool InSync(Note noteLeft, Note noteRight)
        {
            return noteLeft.Updated == noteRight.Updated
                && noteLeft.Pinned == noteRight.Pinned
                && noteLeft.Locked == noteRight.Locked
                && noteLeft.Archived == noteRight.Archived
                && noteLeft.PublicUrl == noteRight.PublicUrl;
        }
    }
}
