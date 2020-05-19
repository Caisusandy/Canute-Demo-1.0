namespace Canute
{
    public interface IExitable
    {
        void Close();
    }

    public interface IOpenable
    {
        void Open();
    }

    public interface IMenu : IOpenable, IExitable
    {
        bool IsForceOpened { get; set; }
    }
}
