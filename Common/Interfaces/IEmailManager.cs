namespace Common.Interfaces
{
    public interface IEmailManager
    {
        void Send(string email, string subject, string body);
    }
}
