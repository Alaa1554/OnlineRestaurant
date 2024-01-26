
using System.Net.Mail;
using System.Net;
using OnlineRestaurant.Interfaces;
using Microsoft.Extensions.Options;
using OnlineRestaurant.Helpers;

namespace OnlineRestaurant.Services
{
    public class EmailSender : IEmailSender
    {     private readonly MessageSender _messageSender;

        public EmailSender(IOptions<MessageSender> messageSender)
        {
            _messageSender = messageSender.Value;
        }

        public void SendEmail(string email, string subject, string htmlMessage)
           {
               var fromemail = _messageSender.Email;
               var frompass = _messageSender.Password;
               var message = new MailMessage();
               message.From = new MailAddress(fromemail);
               message.Subject = subject;
               message.Body = $"<html><body>{htmlMessage}</body></html>";
               message.To.Add(email);
               message.IsBodyHtml = true;
               var smtpclient = new SmtpClient("smtp-relay.brevo.com")
               {
                   Port=587,
                   Credentials=new NetworkCredential(fromemail,frompass),
                   EnableSsl = true
               };
              smtpclient.Send(message);

           }
        
    }
}
