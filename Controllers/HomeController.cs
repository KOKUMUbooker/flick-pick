using Microsoft.AspNetCore.Mvc;
using WatchHive.Services;

namespace WatchHive.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    private IEmailService _emailService;
    private IEmailTemplateService _templateService;
    private IConfiguration _configuration;

    public HomeController(IEmailService emailService, IEmailTemplateService templateService, IConfiguration configuration)
    {
        _emailService = emailService;
        _templateService = templateService;
        _configuration = configuration;
    }

    // [HttpGet("/")]
    // public IActionResult Home()
    // {
    //     return Ok("Hello from movie manager server");
    // }

    [HttpGet("/err")]
    public IActionResult Err()
    {
        int x = 0;
        int n = 500 / x;
        return Ok("Test global error handler");
    }

    [HttpGet("/test-mail")]
    public async Task<IActionResult> SendMail()
    {
        var appName = "Movie-Manager";
        var userName = "Booker";
        string recipientEmail = _configuration.GetValue("EmailConf:TestRecipient", "test@example.com")!;
        string clientUrl = _configuration.GetValue("UIClient:URL", "http://localhost:5173")!;

        var resetEmailUrl = $"{clientUrl}/api/auth/reset-password";
        var verifymailUrl = $"{clientUrl}/api/auth/verify-email?tkn=abcd";

        string htmlBody = false
                ? await _templateService.GeneratePasswordResetEmail(appName, userName, resetEmailUrl)
                : await _templateService.GenerateVerificationEmail(appName, userName, verifymailUrl);

        await _emailService.SendEmail(recipientEmail, "Verify email", htmlBody);

        return Ok(new { message = "Email sent successfully." });
    }
}
