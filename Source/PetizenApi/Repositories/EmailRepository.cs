using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using PetizenApi.Interfaces;
using PetizenApi.Providers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PetizenApi.Repositories
{
    public class EmailRepository : IEmailRepository
    {

        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly EmailConfiguration _emailconfig;
        public EmailRepository(IWebHostEnvironment environment, IConfiguration configuration, IOptions<EmailConfiguration> emailconfig)
        {

            _environment = environment;
            _configuration = configuration;
            if (emailconfig == null) throw new ArgumentNullException(nameof(emailconfig));
            _emailconfig = emailconfig.Value;
        }

        public async Task<bool> ConfirmatonEmailAsync(string Name, string Email, string Link)
        {
            try
            {
                bool isSuccess = false;
                var pathToFile = _environment.WebRootPath
                      + Path.DirectorySeparatorChar.ToString()
                      + "EmailTemplate"
                      + Path.DirectorySeparatorChar.ToString()
                      + "ConfirmatonEmail.html";
                var subject = "Petizen Email Confirmation";

                var builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {
                    builder.HtmlBody = await SourceReader.ReadToEndAsync().ConfigureAwait(false);

                }

                string messageBody = builder.HtmlBody.Replace("@ViewBag.Name", Name, StringComparison.CurrentCulture).Replace("@ViewBag.Link", Link, StringComparison.CurrentCulture);
                builder.HtmlBody = messageBody;

                try
                {
                    isSuccess = true;
                    await SendEmailAsync(Email, subject, builder).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    throw new Exception(e.Message);
                }
                return isSuccess;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<bool> ConfirmEmailFromBackOfficeAsync(string Name, string Email, string Password, string Link)
        {
            try
            {
                bool isSuccess = false;
                var pathToFile = _environment.WebRootPath
                      + Path.DirectorySeparatorChar.ToString()
                      + "EmailTemplate"
                      + Path.DirectorySeparatorChar.ToString()
                      + "ConfirmEmailFromBackOffice.html";
                var subject = "Petizen Email Confirmation";

                var builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {
                    builder.HtmlBody = await SourceReader.ReadToEndAsync().ConfigureAwait(false);

                }

                string messageBody = builder.HtmlBody.Replace("@ViewBag.Name", Name, StringComparison.CurrentCulture).Replace("@ViewBag.Link", Link, StringComparison.CurrentCulture).
                    Replace("@ViewBag.Email", Email, StringComparison.CurrentCulture).Replace("@ViewBag.Password", Password, StringComparison.CurrentCulture);

                builder.HtmlBody = messageBody;

                try
                {
                    isSuccess = true;
                    await SendEmailAsync(Email, subject, builder).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    throw new Exception(e.Message);
                }

                return isSuccess;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<bool> ResetPasswordEmailAsync(string Name, string Email, string Link)
        {
            try
            {
                bool isSuccess = false;
                var pathToFile = _environment.WebRootPath
                      + Path.DirectorySeparatorChar.ToString()
                      + "EmailTemplate"
                      + Path.DirectorySeparatorChar.ToString()
                      + "ResetPasswordEmail.html";
                var subject = "Petizen Reset Password";

                var builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {
                    builder.HtmlBody = await SourceReader.ReadToEndAsync().ConfigureAwait(false);

                }

                string messageBody = builder.HtmlBody.Replace("@ViewBag.Name", Name, StringComparison.CurrentCulture).
                    Replace("@ViewBag.Email", Email, StringComparison.CurrentCulture).Replace("@ViewBag.Link", Link, StringComparison.CurrentCulture);
                builder.HtmlBody = messageBody;

                try
                {
                    isSuccess = true;
                    await SendEmailAsync(Email, subject, builder).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    throw new Exception(e.Message);
                }
                return isSuccess;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }


        public async Task SendEmailAsync(string email, string subject, BodyBuilder message)
        {
            try
            {


                //From Address    
                string FromAddress = _emailconfig.FromAddress;
                string FromAdressTitle = _emailconfig.FromAdressTitle;
                //To Address    
                string ToAddress = email;

                string Subject = subject;
                //Smtp Server    
                string SmtpServer = _emailconfig.SmtpServer;
                //Smtp Port Number    
                int SmtpPortNumber = _emailconfig.SmtpPortNumber;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress
                                        (FromAdressTitle,
                                         FromAddress
                                         ));
                mimeMessage.To.Add(MailboxAddress.Parse
                                         (
                                         ToAddress
                                         ));
                InternetAddressList bccList = new InternetAddressList();

                if (_emailconfig.BccEmail != null && _emailconfig.BccEmail.Count > 0)
                {
                    foreach (var item in _emailconfig.BccEmail)
                    {
                        bccList.Add(MailboxAddress.Parse(item.Trim()));
                    }
                }
                mimeMessage.Bcc.AddRange(bccList);

                InternetAddressList ccList = new InternetAddressList();
                if (_emailconfig.CcEmail != null && _emailconfig.CcEmail.Count > 0)
                {
                    foreach (var item in _emailconfig.CcEmail)
                    {
                        ccList.Add(MailboxAddress.Parse(item.Trim()));
                    }
                }
                mimeMessage.Cc.AddRange(ccList);



                mimeMessage.Subject = Subject; //Subject  
                if (message == null) throw new ArgumentNullException(nameof(message));
                mimeMessage.Body = message.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(SmtpServer, SmtpPortNumber, false).ConfigureAwait(false);
                    await client.AuthenticateAsync(
                        _emailconfig.FromAddress,
                        _emailconfig.Password
                        ).ConfigureAwait(false);

                    await client.SendAsync(mimeMessage).ConfigureAwait(false);

                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
