namespace FlickPickApp.Services;

public interface IEmailService
{
    Task SendEmail(string receptor, string subject, string body);
}

// namespace FlickPickApp.Services;

// public interface IEmailSender
// {
//     Task SendEmail(string senderName,string senderEmail, string toName, string toEmail,string subject, string textContext);
// }
