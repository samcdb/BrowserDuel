using BrowserDuel.Hubs;
using BrowserDuel.Interfaces;
using BrowserDuel.Services;
namespace BrowserDuel;

public class BrowserDuel
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // dependency injection
        builder.Services.AddSingleton<IMatchMakingService, MatchMakingService>();
        builder.Services.AddSingleton<IAccountRepo, AccountRepo>();
        builder.Services.AddControllers();
        builder.Services.AddSignalR();

        string corsPolicy = "BrowserDuel";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicy,
               builder =>
               {
                   builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
               });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseCors(corsPolicy);
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ChatHub>("/chatHub");
        app.MapHub<MatchHub>("/matchHub");

        app.Run();
    }
}