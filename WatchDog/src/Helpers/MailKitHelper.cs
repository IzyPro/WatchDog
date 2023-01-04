using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog.src.Helpers
{
    public class MailKitConfig
    {
        public bool enableMailKit { get; set; } = true;
        public MailKitClient mailKitClient { get; set; }
        public MailSetting mailSetting { get; set; }
    }
    public class MailKitClient
    {
        public string hostUrl { get; set; }
        public int port { get; set; }
        public bool useSsl { get; set; }
        public string account { get; set; }
        public string password { get; set; }
    }
    public class MailSetting
    {
        public string senderName { get; set; }
        public string senderAddress { get; set; }
        public string receiverName { get; set; }
        public string receiverAddress { get; set; }
        public string subject { get; set; }
        public Exception? exception { get; set; }
    }
    public class MailKitHelper
    {
        private static SmtpClient _client;
        public MailKitHelper(MailKitClient mailKitClient)
        {
            MailKitInit(mailKitClient);
        }
       
        public void SendMail(MailSetting setting)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(setting.senderName, setting.senderAddress));
            message.To.Add(new MailboxAddress(setting.receiverName, setting.receiverAddress));
            message.Subject = setting.subject;

            var bodyBuilder = new BodyBuilder();

            while (setting.exception!=null)
            {
                bodyBuilder.HtmlBody += $@"<h5>{setting.exception.Message}</h5><p>{setting.exception.StackTrace}</p><br><hr>";
                setting.exception = setting.exception.InnerException;
            }

            message.Body = bodyBuilder.ToMessageBody();

            _client.Send(message);
            _client.Disconnect(true);
        }
        private void MailKitInit(MailKitClient mailKitClient)
        {
            _client = new SmtpClient();
            _client.Connect(mailKitClient.hostUrl, mailKitClient.port, mailKitClient.useSsl);
            _client.Authenticate(mailKitClient.account, mailKitClient.password);
        }
    }
}
