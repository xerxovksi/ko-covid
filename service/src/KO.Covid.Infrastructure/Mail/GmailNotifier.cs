namespace KO.Covid.Infrastructure.Mail
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;

    public class GmailNotifier : INotifier
    {
        private const int BatchSize = 5;

        private readonly SmtpClient gmailClient = null;
        private readonly string from = null;

        public GmailNotifier(SmtpClient gmailClient, string from)
        {
            this.gmailClient = gmailClient;
            this.from = from;
        }

        public async Task SendAsync(
            string subject,
            List<string> recepients,
            string message)
        {
            var batches = recepients.GetBatches(BatchSize);
            foreach (var batch in batches)
            {
                var operation = batch.Select(recepient =>
                {
                    var mailMessage = new MailMessage(
                        from: this.from,
                        to: recepient,
                        subject: subject,
                        body: message);

                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.IsBodyHtml = true;

                    return gmailClient.SendMailAsync(mailMessage);
                });

                await Task.WhenAll(operation);
            }
        }
    }
}
