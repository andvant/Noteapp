namespace Noteapp.Core.Dtos
{
    public class UpdateNoteDtoNew
    {
        public string Text { get; set; }
        public bool Locked { get; set; }
        public bool Archived { get; set; }
        public bool Pinned { get; set; }
        public bool Published { get; set; }
    }
}
