namespace FlickPickApp.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IWebHostEnvironment _environment;

    public EmailTemplateService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    private async Task<string> LoadTemplateAsync(string templateName)
    {
        var filePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", templateName);

        if (File.Exists(filePath))
        {
            return await File.ReadAllTextAsync(filePath);
        }

        // Fallback to embedded simple template if file not found
        return templateName switch
        {
            "EmailVerificationTemplate.html" => GetFallbackVerificationTemplate(),
            "PasswordResetTemplate.html" => GetFallbackPasswordResetTemplate(),
            _ => throw new FileNotFoundException($"Template not found: {templateName}")
        };
    }

    public async Task<string> GenerateVerificationEmail(string appName, string userName, string verificationUrl)
    {
        var template = await LoadTemplateAsync("EmailVerificationTemplate.html");

        return template
            .Replace("{{AppName}}", appName)
            .Replace("{{UserName}}", userName)
            .Replace("{{VerificationUrl}}", verificationUrl)
            .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());
    }

    public async Task<string> GeneratePasswordResetEmail(string appName, string userName, string resetUrl)
    {
        var template = await LoadTemplateAsync("PasswordResetTemplate.html");

        return template
            .Replace("{{AppName}}", appName)
            .Replace("{{UserName}}", userName)
            .Replace("{{ResetUrl}}", resetUrl)
            .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());
    }

    // Fallback templates in case HTML files are not found
    private static string GetFallbackVerificationTemplate()
    {
        return @"<!DOCTYPE html><html><body>
            <h2>Verify Your Email</h2>
            <p>Hi {{UserName}},</p>
            <p>Welcome to {{AppName}}! Please verify your email by clicking: <a href='{{VerificationUrl}}'>{{VerificationUrl}}</a></p>
            <p>This link expires in 24 hours.</p>
            <p>&copy; {{CurrentYear}} {{AppName}}</p>
        </body></html>";
    }

    private static string GetFallbackPasswordResetTemplate()
    {
        return @"<!DOCTYPE html><html><body>
            <h2>Reset Your Password</h2>
            <p>Hi {{UserName}},</p>
            <p>Reset your {{AppName}} password by clicking: <a href='{{ResetUrl}}'>{{ResetUrl}}</a></p>
            <p>This link expires in 1 hour.</p>
            <p>&copy; {{CurrentYear}} {{AppName}}</p>
        </body></html>";
    }
}
