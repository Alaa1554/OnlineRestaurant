using System.Net.Mail;
using System.Net;
using OnlineRestaurant.Interfaces;
using Microsoft.Extensions.Options;
using OnlineRestaurant.Helpers;

namespace OnlineRestaurant.Services
{
    public class EmailSender : IEmailSender
    {    
        private readonly MessageSender _messageSender;

        public EmailSender(IOptions<MessageSender> messageSender)
        {
            _messageSender = messageSender.Value;
        }

        public void SendEmail(string email, string subject, string htmlMessage)
           {
               var fromEmail = _messageSender.Email;
               var fromPass = _messageSender.Password;
               var message = new MailMessage();
               message.From = new MailAddress(fromEmail);
               message.Subject = subject;
               message.Body = $"<html><body>{htmlMessage}</body></html>";
               message.To.Add(email);
               message.IsBodyHtml = true;
               var smtpClient = new SmtpClient("smtp-relay.brevo.com")
               {
                   Port=587,
                   Credentials=new NetworkCredential(fromEmail,fromPass),
                   EnableSsl = true
               };
              smtpClient.Send(message);

           }
        
    }
}
