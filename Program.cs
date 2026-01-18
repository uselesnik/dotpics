using DotPic.Models;
using DotPic.Services;
using StackExchange.Redis;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MongoDB configuration
var mongoSettings = new MongoDbSettings
{
    ConnectionString = builder.Configuration["MongoDbSettings:ConnectionString"] ?? "mongodb://localhost:27017",
    DatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"] ?? "DotPicDb"
};

// Redis configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConn = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    var options = ConfigurationOptions.Parse(redisConn);
    options.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(options);
});

// Configure data protection to use Redis (shared across pods)
builder.Services.AddDataProtection()
    .SetApplicationName("DotPics") // important when multiple apps/pods share Redis
    .PersistKeysToStackExchangeRedis(
        () => ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379").GetDatabase(),
        "DataProtection-Keys");


builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IImageService, ImageService>(); 
builder.Services.AddSingleton<IRedisService, RedisService>(); // Add Redis service

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
// Only enable HTTPS redirection when the configured URLs include an HTTPS endpoint.
// This prevents Kestrel trying to bind an HTTPS endpoint without a certificate (e.g. in containers behind nginx).
var configuredUrls = builder.Configuration["ASPNETCORE_URLS"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (!string.IsNullOrEmpty(configuredUrls) && configuredUrls.Contains("https", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<DotPic.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();