using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace WatchHive.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmail(string receptor, string subject, string htmlBody)
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
            From = new MailAddress(email!, "Watch Hive"),
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
}

// using System.Threading.Tasks;
// using MailKit.Net.Smtp;
// using MimeKit;

// namespace WatchHive.Services;

// public class EmailSender : IEmailSender
// {
//     private readonly string?  SmtpServer;
//     private readonly int SmtpPort;
//     private readonly string?  SmtpUsername;
//     private readonly string?  SmtpPassword;

//     public EmailSender(IConfiguration configuration)
//     {
//         SmtpServer = configuration.GetValue<string>("SmtpSettings:SmtpServer","");
//         SmtpPort = configuration.GetValue<int>("SmtpSettings:SmtpPort",0);
//         SmtpUsername = configuration.GetValue<string>("SmtpSettings:SmtpUsername","");
//         SmtpPassword = configuration.GetValue<string>("SmtpSettings:SmtpPassword","");
//     }

//     public async Task SendEmail(
//         string senderName,
//         string senderEmail,
//         string toName,
//         string toEmail,
//         string subject,
//         string textContext)
//     {
//         var message = new MimeMessage();
//         message.From.Add(new MailboxAddress(senderName, senderEmail));
//         message.To.Add(new MailboxAddress(toName, toEmail));
//         message.Subject = subject;

//         message.Body = new TextPart("plain")
//         {
//             Text = textContext
//         };

//         using var client = new SmtpClient();

//         Console.WriteLine($"Connecting to {SmtpServer}:{SmtpPort}");

//         await client.ConnectAsync(
//             SmtpServer,
//             SmtpPort,
//             MailKit.Security.SecureSocketOptions.StartTls
//         );

//         Console.WriteLine("Connected");

//         await client.AuthenticateAsync(SmtpUsername, SmtpPassword);

//         Console.WriteLine("Authenticated");

//         await client.SendAsync(message);
//         await client.DisconnectAsync(true);
//     }
// }
