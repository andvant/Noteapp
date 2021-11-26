namespace Noteapp.Core.Exceptions
{
    public class SnapshotNotFoundException : NoteappException
    {
        public SnapshotNotFoundException(int noteId, int snapshotId)
            : base($"The note with id {noteId} does not have a snapshot with id {snapshotId}")
        {
        }
    }
}
