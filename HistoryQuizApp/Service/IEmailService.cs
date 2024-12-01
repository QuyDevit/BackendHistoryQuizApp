namespace HistoryQuizApp.Service
{
    public interface IEmailService
    {
        void SendEmail(string recipientEmail, string subject, string message);
    }
}
