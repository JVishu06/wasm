using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using wasm;
using wasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Authorization services
builder.Services.AddAuthorizationCore();

// Register Root Components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register WeatherService and LocalStorage services
builder.Services.AddScoped<WeatherService>();
builder.Services.AddBlazoredLocalStorage();

// Add Custom Authentication and HTTP Handler services
builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

// Register HttpClient services
var backendUrl = builder.Configuration["BackendUrl"] ?? "https://webapi-8j7b.onrender.com";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(backendUrl)
});

// Add HTTP Client for authentication (if needed)
builder.Services.AddHttpClient("Auth", opt => opt.BaseAddress = new Uri(backendUrl))
    .AddHttpMessageHandler<CustomHttpHandler>();

await builder.Build().RunAsync();
