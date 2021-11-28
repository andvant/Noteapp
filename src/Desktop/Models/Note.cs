using Noteapp.Desktop.MVVM;
using System;

namespace Noteapp.Desktop.Models
{
    public class Note : NotifyPropertyChanged
    {
        private string _text;

        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Text 
        { 
            get => _text;
            set => Set(ref _text, value);
        }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool Locked { get; set; }
        public bool Archived { get; set; }
        public bool Pinned { get; set; }
        public string PublicUrl { get; set; }
        public bool Published => PublicUrl != null;
    }
}
