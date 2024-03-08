using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddPolicy("CORS_ALLOW_ALL", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

builder.Services.AddControllers();
builder.Services.AddAuthentication(o =>
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
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
