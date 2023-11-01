using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using DartRad.Configurations;
using System.Net.Security;
using Org.BouncyCastle.Tls;
using System.Security.Cryptography.X509Certificates;

namespace DartRad.Services
{
    public class GmailService : IMailService
    {
        private readonly GmailConfig _config;
        public GmailService(IOptions<GmailConfig> options)
        {
            _config = options.Value;
        }
        public void Send(EMailMessage model)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("DartRad", _config.Username));
            emailMessage.To.Add(new MailboxAddress(model.ReceiverName, model.ReceiverEmail));
            emailMessage.Subject = model.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = model.Body;

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;

                // if there are errors in the certificate chain, look at each error to determine the cause.
                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                {
                    if (chain != null && chain.ChainStatus != null)
                    {
                        foreach (var status in chain.ChainStatus)
                        {
                            if ((cert.Subject == cert.Issuer) && (status.Status == X509ChainStatusFlags.UntrustedRoot))
                            {
                                // self-signed certificates with an untrusted root are valid. 
                                continue;
                            }
                            else if (status.Status != X509ChainStatusFlags.NoError)
                            {
                                // if there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }

                    // When processing reaches this line, the only errors in the certificate chain are 
                    // untrusted root errors for self-signed certificates. These certificates are valid
                    // for default Exchange server installations, so return true.
                    return true;
                }

                return false;
            };
            try
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_config.Username, _config.Password);
                client.Send(emailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

        }
    }
}
