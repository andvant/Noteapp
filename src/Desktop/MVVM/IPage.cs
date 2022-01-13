namespace Noteapp.Desktop.MVVM
{
    public interface IPage
    {
        string Name { get; }
        void RefreshPage();
    }
}
