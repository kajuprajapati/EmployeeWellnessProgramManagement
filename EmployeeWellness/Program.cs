using EmployeeWellness.Core;
using EmployeeWellness.Core.Data;
using EmployeeWellness.Core.Interfaces;
using EmployeeWellness.Core.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<EmployeeWellnessContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeWelnessConnectionString")));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));


builder.Services.AddSingleton(sp => {
    var uri = builder.Configuration.GetValue<string>("RabbitMQ:Uri") ?? "amqp://guest:guest@localhost:5672/";
    return new ConnectionFactory() { Uri = new Uri(uri) };
});

builder.Services.AddHostedService<ProgressWorker>();

// Register DI services
builder.Services.AddScoped<IChallengeService, ChallengeService>();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.MapControllers();

// Automatically apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeWellnessContext>();
    dbContext.Database.Migrate();  // <-- applies any pending migrations
}

app.Run();