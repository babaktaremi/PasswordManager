using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PasswordManager.Database.PasswordManager;
using PasswordManager.Database.PasswordManager.Models;
using PasswordManager.Database.PasswordManager.Services;
using PasswordManager.Database.Tenet;
using PasswordManager.Database.Tenet.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPasswordManagerTenetService, PasswordManagerTenetService>();


builder.Services.AddDbContext<TenetManagerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TenentManagerDb")).EnableSensitiveDataLogging(true);
});

builder.Services.AddDbContext<PasswordManagerDbContext>((serviceProvider, options) =>
{

    var passwordManagerTenetService = serviceProvider.GetRequiredService<IPasswordManagerTenetService>();

    var connectionString = string.Format(builder.Configuration.GetConnectionString("PasswordManagerDb")!, passwordManagerTenetService.GetUserTenetId());

    options.UseSqlServer(connectionString).EnableSensitiveDataLogging(true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/SpecifiyUserTenet",
    async (TenetManagerDbContext tenetManagerDbContext
        , IServiceProvider serviceProvider) =>
    {
        var userTenet = new TenetUser(new TenantId(Guid.NewGuid(),DateTime.Now));
        tenetManagerDbContext.TenetUsers.Add(userTenet);
        await tenetManagerDbContext.SaveChangesAsync();

        using var serviceScope = serviceProvider.CreateScope();

        var passwordManager = serviceScope.ServiceProvider.GetRequiredService<IPasswordManagerTenetService>();
        passwordManager.SetUserTenetId(userTenet.Id.Id);

        var passwordManagerDbContext = serviceScope.ServiceProvider.GetRequiredService<PasswordManagerDbContext>();
        await passwordManagerDbContext.Database.MigrateAsync();

        return Results.Ok(userTenet);
    });

app.MapPost("/AddPassword",
    async (AddPasswordApiModel addPasswordApiModel, [FromHeader(Name = "UserId")] TenantId? userId
        , IServiceProvider serviceProvider
        , TenetManagerDbContext tenetUserManager) =>
    {
        if (userId ==null)
            return Results.NotFound("User Id not found");
        var users=await tenetUserManager.TenetUsers.ToListAsync();
        var tenetUser = await tenetUserManager.TenetUsers.FirstOrDefaultAsync(c => c.Id == userId);

        if (tenetUser is null)
            return Results.NotFound("Specified Tenet User Not Found");

        using var serviceScope = serviceProvider.CreateScope();

        var passwordManagerTenet = serviceScope.ServiceProvider.GetRequiredService<IPasswordManagerTenetService>();

        passwordManagerTenet.SetUserTenetId(tenetUser.Id.Id);

        var passwordManagerDbContext = serviceScope.ServiceProvider.GetRequiredService<PasswordManagerDbContext>();

        var password = new UserPassword(Guid.NewGuid(), addPasswordApiModel.Password);
        passwordManagerDbContext.UserPasswords.Add(password);
        await passwordManagerDbContext.SaveChangesAsync();
        return Results.Ok(password);
    });

app.MapGet("/GetPasswords",
    async ( [FromHeader(Name = "UserId")] Guid userId
        , TenetManagerDbContext tenetUserManager
        ,IServiceProvider serviceProvider) =>
    {
        if (userId == Guid.Empty)
            return Results.NotFound("User Id not found");

        var tenetUser = await tenetUserManager.TenetUsers.FirstOrDefaultAsync(c => c.Id.Id == userId);

        if (tenetUser is null)
            return Results.NotFound("Specified Tenet User Not Found");

        using var serviceScope = serviceProvider.CreateScope();

        var passwordManagerTenet = serviceScope.ServiceProvider.GetRequiredService<IPasswordManagerTenetService>();

        passwordManagerTenet.SetUserTenetId(tenetUser.Id.Id);

        var passwordManagerDbContext = serviceScope.ServiceProvider.GetRequiredService<PasswordManagerDbContext>();

        passwordManagerTenet.SetUserTenetId(tenetUser.Id.Id);

        var passwords = await passwordManagerDbContext.UserPasswords.ToListAsync();
        return Results.Ok(passwords);
    });

app.Run();


public record AddPasswordApiModel(string Password);