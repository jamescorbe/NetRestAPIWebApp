using BlazorAppTest2.Components;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using BlazorAppTest_Client;
using BlazorAppTest2.Helpers;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// temp solution move to sqlserver db
var authHelper = new AuthHelper(builder.Configuration.GetValue<string>("AuthLoc"), builder.Configuration.GetValue<string>("AppID"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

})
.AddCookie()
// should move this to a better location + encrypted when doing it properly not in appsettings
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = authHelper.GoogleAuth.ClientID;
    options.ClientSecret = authHelper.GoogleAuth.ClientSecret;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

//https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
builder.Services.AddHttpClient<SigninClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetValue<string>("RestApiAddress"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
