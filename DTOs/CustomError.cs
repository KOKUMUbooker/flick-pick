namespace WatchHive.DTOs;

public enum AuthErrorType
{
    None,
    InvalidCredentials,
    EmailNotVerified,
    InvalidClient
}


public class CustomError
{
    public string? Error { get; set; } = "ERROR";
    public required string Message { get; set; } = String.Empty;
}
