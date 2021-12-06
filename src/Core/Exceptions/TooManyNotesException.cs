namespace Noteapp.Core.Exceptions
{
    public class TooManyNotesException : NoteappException
    {
        public TooManyNotesException(int actualCount, int maxCount)
            : base($"Tried to create {actualCount} notes, but the allowed limit is {maxCount}")
        {
        }

        public static void ThrowIfTooManyNotes(int actualCount, int maxCount)
        {
            if (actualCount > maxCount)
            {
                throw new TooManyNotesException(actualCount, maxCount);
            }
        }
    }
}