using aspnetcore_web_api.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddCors(o => o.AddPolicy("CORS_ALLOW_ALL", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

services.AddControllers();
services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(g =>
    {
        g.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        g.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        g.SaveTokens = true;
    })
    .AddMicrosoftAccount(ms =>
    {
        ms.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        ms.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
        ms.SaveTokens = true;
    });
//.AddFacebook(fb =>
//{
//    fb.AppId = builder.Configuration.GetValue<string>("Authentication:Facebook:AppId");
//    fb.AppSecret = builder.Configuration.GetValue<string>("Authentication:Facebook:AppSecret");
//    fb.SaveTokens = true;
//})
//.AddApple(a =>
//{
//    a.ClientId = builder.Configuration.GetValue<string>("Authentication:Apple:ClientId");
//    a.KeyId = builder.Configuration.GetValue<string>("Authentication:Apple:KeyId");
//    a.TeamId = builder.Configuration.GetValue<string>("Authentication:Apple:TeamId");
//    a.UsePrivateKey(keyId => WebHostEnvironment.ContentRootFileProvider.GetFileInfo($"AuthKey_{keyId}.p8"));
//    a.SaveTokens = true;
//});

//Cookie Policy needed for External Auth
services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
});

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("AppDb"));

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.MapIdentityApi<IdentityUser>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("CORS_ALLOW_ALL");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
