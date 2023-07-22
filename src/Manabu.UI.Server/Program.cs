using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Manabu.UI.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.Configure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.ClientSecret = Environment.GetEnvironmentVariable("ManabuServerClientSecret");
        options.Scope.Add("offline_access");
        options.Scope.Add(options?.ClientId);
        options.Events.OnRedirectToIdentityProvider = async context =>
        {
            if (builder.Environment.IsProduction())
                context.ProtocolMessage.RedirectUri = "https://manabu.pl/signin-oidc";
        }; 
        options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
        {
            if (builder.Environment.IsProduction())
                context.ProtocolMessage.PostLogoutRedirectUri = "https://manabu.pl/";
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
builder.Services.AddHttpContextAccessor();

builder.Services.InitializeApp(builder.Environment);

var app = builder.Build();

app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseCookiePolicy();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
