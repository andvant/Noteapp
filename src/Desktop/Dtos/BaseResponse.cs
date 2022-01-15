namespace Noteapp.Desktop.Dtos
{
    public abstract class BaseResponse
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    }
}
