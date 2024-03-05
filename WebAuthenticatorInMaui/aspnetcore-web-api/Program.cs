using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddFacebook(fb =>
    {
        fb.AppId = builder.Configuration.GetValue<string>("FacebookAppId");
        fb.AppSecret = builder.Configuration.GetValue<string>("FacebookAppSecret");
        fb.SaveTokens = true;
    })
    .AddGoogle(g =>
    {
        g.ClientId = builder.Configuration.GetValue<string>("GoogleClientId");
        g.ClientSecret = builder.Configuration.GetValue<string>("GoogleClientSecret");
        g.SaveTokens = true;
    })
    .AddMicrosoftAccount(ms =>
    {
        ms.ClientId = builder.Configuration.GetValue<string>("MicrosoftClientId");
        ms.ClientSecret = builder.Configuration.GetValue<string>("MicrosoftClientSecret");
        ms.SaveTokens = true;
    })
    .AddApple(a =>
    {
        a.ClientId = builder.Configuration.GetValue<string>("AppleClientId");
        a.KeyId = builder.Configuration.GetValue<string>("AppleKeyId");
        a.TeamId = builder.Configuration.GetValue<string>("AppleTeamId");
        a.UsePrivateKey(keyId => WebHostEnvironment.ContentRootFileProvider.GetFileInfo($"AuthKey_{keyId}.p8"));
        a.SaveTokens = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
