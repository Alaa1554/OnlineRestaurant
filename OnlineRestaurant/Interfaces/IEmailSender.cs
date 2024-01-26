namespace OnlineRestaurant.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string email, string subject, string htmlMessage);
    }
}
