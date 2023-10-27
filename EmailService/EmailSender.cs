using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailSender(EmailConfiguration emailConfiguration) {
            _emailConfiguration = emailConfiguration;
        }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateMimeMessage(message);
            SendMimeMessage(emailMessage);

        }

        private MimeMessage CreateMimeMessage(Message message)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfiguration.UserName, _emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };


            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Follow this link: {message.Content}"
                               
            };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes);
                }
                emailMessage.Body = bodyBuilder.ToMessageBody();
                return emailMessage;
            }

            emailMessage.Body = bodyBuilder.ToMessageBody() ;
            return emailMessage;
        }

        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateMimeMessage(message);
            await SendMimeMessageAsync(emailMessage);
        }

        private async Task SendMimeMessageAsync(MimeMessage mimeMessage)
        {
            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                   await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                   await client.AuthenticateAsync(_emailConfiguration.From, _emailConfiguration.Password);

                    await client.SendAsync(mimeMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private void SendMimeMessage(MimeMessage mimeMessage) 
        {
            using (SmtpClient client = new SmtpClient()) {
                try
                {
                    client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfiguration.From, _emailConfiguration.Password);

                    client.Send(mimeMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
