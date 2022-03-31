using AdCore.Middleware;
using AdCore.Repository;
using AdCore.Service;
using AdCore.Store;
using AdUi;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using RestSharp;
using Syncfusion.Blazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("Syncfusion"));
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddHttpClient("AuthorizeApi", client =>
        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiBaseUrl")))
    .AddHttpMessageHandler(sp =>
    {
        var handler = sp.GetService<AuthorizationMessageHandler>()!
            .ConfigureHandler(
                authorizedUrls: new[] { builder.Configuration.GetValue<string>("ApiBaseUrl") }, //<--- The URI used by the Server project.
                scopes: new[] { builder.Configuration.GetValue<string>("AzureAdB2C:Scope") });
        return handler;
    });
builder.Services.AddHttpClient("UnAuthorizeApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiBaseUrl"));
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("AuthorizeApi").EnableIntercept(sp));

builder.Services.AddSingleton(sp => 
    new RestClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("UnAuthorizeApi").EnableIntercept(sp)));

builder.Services.AddLoadingBar();
builder.Services.AddHttpClientInterceptor();
builder.Services.AddScoped<HttpInterceptorService>();

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
}).AddAccountClaimsPrincipalFactory<CustomAccountClaimsPrincipalFactory>();

builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });

//global services
builder.Services.AddSingleton<ToastService>();
builder.Services.AddSingleton<StoreContainer>();
builder.Services.AddScoped<BaseHttpClient>();
builder.Services.AddSingleton(services =>
    (IJSInProcessRuntime)services.GetRequiredService<IJSRuntime>());

builder.UseLoadingBar();

await builder.Build().RunAsync();
