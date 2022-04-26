using BrowserDuel.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();

string corsPolicy = "BrowserDuel";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
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

app.Run();
