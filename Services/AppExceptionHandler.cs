using Microsoft.AspNetCore.Diagnostics;
using WatchHive.DTOs;

namespace WatchHive.Services;

public class AppExceptionHandler : IExceptionHandler
{

    private readonly IHostEnvironment _env;

    public AppExceptionHandler(IHostEnvironment env )
    {
        _env = env;
    }
   
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var response = new CustomError
        {
            Message = _env.IsDevelopment() ? exception.Message : "Something went wrong"
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}