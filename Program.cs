using DotPic.Models;
using DotPic.Services;

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

builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<DotPic.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();