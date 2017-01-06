using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using The_World.ViewModels;

namespace The_World.Services
{
    public class MailService : IMailService
    {
        private IConfigurationRoot _config;

        public MailService(IConfigurationRoot config)
        {
            _config = config;
        }
        public async Task SendMail(ContactViewModel model)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(model.Email));
            emailMessage.To.Add(new MailboxAddress("", _config["MailSettings:ToAddress"]));
            emailMessage.Subject = "The World: New Query";
            emailMessage.Body = new TextPart("plain") { Text = model.Message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 25, SecureSocketOptions.None).ConfigureAwait(false);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
