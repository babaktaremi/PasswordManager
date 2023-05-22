using Microsoft.EntityFrameworkCore;
using PasswordManager.Database.PasswordManager;
using PasswordManager.Database.PasswordManager.Models;
using PasswordManager.Database.PasswordManager.Services;
using PasswordManager.Database.Tenet;
using PasswordManager.Database.Tenet.Models;
using PasswordManager.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPasswordManagerTenetService, PasswordManagerTenetService>();

builder.Services.AddScoped<SetUserTenetNodeMiddleware>();

builder.Services.AddDbContext<TenetManagerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TenentManagerDb")).EnableSensitiveDataLogging(true);
});

builder.Services.AddDbContext<PasswordManagerDbContext>((serviceProvider, options) =>
{
   
    var passwordManagerTenetService=serviceProvider.GetRequiredService<IPasswordManagerTenetService>();

    var connectionString=string.Format(builder.Configuration.GetConnectionString("PasswordManagerDb")!,passwordManagerTenetService.GetUserTenetId());

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

app.UseMiddleware<SetUserTenetNodeMiddleware>();

app.MapPost("/SpecifiyUserTenet",
    async (TenetManagerDbContext tenetManagerDbContext
        ,IServiceProvider serviceProvider) =>
    {
        var userTenet = new TenetUser(Guid.NewGuid());
        tenetManagerDbContext.TenetUsers.Add(userTenet);
        await tenetManagerDbContext.SaveChangesAsync();

        using var serviceScope = serviceProvider.CreateScope();

        var passwordManager=serviceScope.ServiceProvider.GetRequiredService<IPasswordManagerTenetService>();
        passwordManager.SetUserTenetId(userTenet.Id);

        var passwordManagerDbContext = serviceScope.ServiceProvider.GetRequiredService<PasswordManagerDbContext>(); 
        await passwordManagerDbContext.Database.MigrateAsync(); 

        return Results.Ok(userTenet);
    });

app.MapPost("/AddPassword",
    (AddPasswordApiModel addPasswordApiModel, PasswordManagerDbContext passwordManagerDbContext) =>
    {
    var password=new UserPassword(Guid.NewGuid(), addPasswordApiModel.Password);
    passwordManagerDbContext.UserPasswords.Add(password);
    passwordManagerDbContext.SaveChanges();
    return Results.Ok(password);
});

app.MapGet("/GetPasswords",
    async (PasswordManagerDbContext passwordManagerDbContext) =>
    {
        var passwords=await passwordManagerDbContext.UserPasswords.ToListAsync();
        return Results.Ok(passwords);
    });

app.Run();


public record AddPasswordApiModel(string Password);