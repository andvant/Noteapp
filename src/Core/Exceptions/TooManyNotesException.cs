namespace Noteapp.Core.Exceptions
{
    public class TooManyNotesException : NoteappException
    {
        public TooManyNotesException(int actualCount, int maxCount)
            : base($"Tried to create {actualCount} notes, but the maximum of {maxCount} is allowed")
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