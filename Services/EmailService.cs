using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Resend;

namespace WatchHive.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IResend _resend;

    public EmailService(IConfiguration configuration, IResend resend)
    {
        _configuration = configuration;
        _resend = resend;
    }

    public async Task SendEmail(string receptor, string subject, string htmlBody)
    {
        var env = _configuration.GetValue<string>("ASPNETCORE:ENVIRONMENT");
        if (env == "Production")
        {
            await SendEmailPrd(receptor,subject,htmlBody);
        } else
        {
            await SendEmailDev(receptor,subject,htmlBody);
        }
    }
    public async Task SendEmailDev(string receptor, string subject, string htmlBody)
    {
        var email = _configuration.GetValue<string>("EmailConf:Email");
        var password = _configuration.GetValue<string>("EmailConf:Password");
        var host = _configuration.GetValue<string>("EmailConf:Host");
        var port = _configuration.GetValue<int>("EmailConf:Port");

        var smtpClient = new SmtpClient(host, port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(email, password)
        };

        var message = new MailMessage
        {
            From = new MailAddress(email!, "WatchHive"),
            Subject = subject,
        };

        message.To.Add(receptor);

        var htmlView = AlternateView.CreateAlternateViewFromString(
            htmlBody,
            null,
            MediaTypeNames.Text.Html
        );

        message.AlternateViews.Add(htmlView);

        await smtpClient.SendMailAsync(message);
    }

    public async Task SendEmailPrd(string receptor, string subject, string htmlBody)
    {
        var from = _configuration["EmailConf:From"];

        var message = new EmailMessage
        {
            From = from ?? throw new Exception("EmailConf:From not configured"),
            To = new EmailAddressList {receptor},
            Subject = subject,
            HtmlBody = htmlBody
        };
        
        await _resend.EmailSendAsync( message );
    }
}
