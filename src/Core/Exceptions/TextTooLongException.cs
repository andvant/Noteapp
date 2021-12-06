namespace Noteapp.Core.Exceptions
{
    public class TextTooLongException : NoteappException
    {
        public TextTooLongException(int actualLength, int maxLength)
            : base($"Tried to update a note with the text of length {actualLength}, but the allowed limit is {maxLength}")
        {
        }

        public static void ThrowIfTextTooLong(int actualLength, int maxLength)
        {
            if (actualLength > maxLength)
            {
                throw new TextTooLongException(actualLength, maxLength);
            }
        }
    }
}
