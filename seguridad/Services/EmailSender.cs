using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace seguridad.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        IConfiguration configuration;
        public EmailSender(IConfiguration config)
        {
            this.configuration = config;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {

           var server=  configuration["SmtpSettings:Server"];

            configuration.GetSection("SmtpSettings").Bind(new Object());
            //var stmp = new SmtpClient();
            //var mail = new MailMessage();
           

            //  stmp.Send();


            return Task.CompletedTask;
        }
    }
}
