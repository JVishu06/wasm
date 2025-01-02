using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using wasm;
using wasm.Sevices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add AuthorizationCore services
builder.Services.AddAuthorizationCore();

// Root components registration
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Local Storage Service
builder.Services.AddBlazoredLocalStorage();

// Custom authentication state provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

// HTTP client configuration for backend API
var backendUrl = builder.Configuration["BackendUrl"] ?? "https://webapi-8j7b.onrender.com";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(backendUrl)
});

// WeatherService registration
builder.Services.AddScoped<WeatherService>();

// Custom HTTP message handler for authentication
builder.Services.AddTransient<CutomHttpHandler>();

// Register HTTP client for authentication
builder.Services.AddHttpClient("Auth", client =>
{
    client.BaseAddress = new Uri(backendUrl);
}).AddHttpMessageHandler<CutomHttpHandler>();

await builder.Build().RunAsync();
