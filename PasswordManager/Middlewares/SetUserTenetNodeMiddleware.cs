using Microsoft.EntityFrameworkCore;
using PasswordManager.Database.PasswordManager.Services;
using PasswordManager.Database.Tenet;

namespace PasswordManager.Middlewares;

public class SetUserTenetNodeMiddleware:IMiddleware
{
    private readonly IPasswordManagerTenetService _passwordManagerTenetService;
    private readonly TenetManagerDbContext _tenetManagerDbContext;

    public SetUserTenetNodeMiddleware(IPasswordManagerTenetService passwordManagerTenetService, TenetManagerDbContext tenetManagerDbContext)
    {
        _passwordManagerTenetService = passwordManagerTenetService;
        _tenetManagerDbContext = tenetManagerDbContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("SpecifiyUserTenet"))
        {
            await next(context);
        }
        else
        {
            var userTenetId = context.Request.Headers.Where(c => c.Key == "userId")
                .Select(c => c.Value).Select(c => c.ToString())
                .Select(Guid.Parse).FirstOrDefault();

            if (userTenetId == Guid.Empty)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { Message = "User Id not found" });
                return;
            }

            var userTenet = await _tenetManagerDbContext
                .TenetUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == userTenetId);

            if (userTenet is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { Message = "Invalid User Tenet Id" });
                return;
            }

            _passwordManagerTenetService.SetUserTenetId(userTenet.Id);

            await next(context);
        }

        
    }
}