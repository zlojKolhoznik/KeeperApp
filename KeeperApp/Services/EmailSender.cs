using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace KeeperApp.Services
{
    /// <summary>
    /// Ideally, this class should be implemented on the server side. 
    /// However, since the server side is not implemented in this project, this class is implemented on the client side.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// On the server side, the email server settings and credentials should be stored in a secure way, such as app secrets and so, there would be no need for this constructor.
        /// </summary>
        public EmailSender()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var login = localSettings.Values["EmailLogin"];
            var password = localSettings.Values["EmailPassword"];
            var smtpHost = localSettings.Values["SmtpHost"];
            var smtpPort = localSettings.Values["SmtpPort"];
            if (login is null || password is null || smtpHost is null || smtpPort is null)
            {
                localSettings.Values["EmailLogin"] = "rportyanko10@gmail.com";
                localSettings.Values["EmailPassword"] = "escwrevulfsjrewh";
                localSettings.Values["SmtpHost"] = "smtp.gmail.com";
                localSettings.Values["SmtpPort"] = 465;
            }
        }

        public async Task SendEmailAsync(string email, string subject, string text)
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress("KeeperApp", "noreply@mail.keeperapp.net"));
            mime.To.Add(new MailboxAddress("", email));
            mime.Subject = subject;
            mime.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = text
            };
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            using var client = new SmtpClient();
            string host = (string)settings.Values["SmtpHost"];
            int port = (int)settings.Values["SmtpPort"];
            string login = (string)settings.Values["EmailLogin"];
            string password = (string)settings.Values["EmailPassword"];
            await client.ConnectAsync(host, port, true);
            await client.AuthenticateAsync(login, password);
            await client.SendAsync(mime);
        }
    }
}
