using wasm;
using wasm.Sevices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
builder.Services.AddTransient<CutomHttpHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

// Register HttpClient services with Render API URL
// Use a dynamic base address for HttpClient
var baseAddress = builder.HostEnvironment.IsProduction()
    ? "https://webapi-8j7b.onrender.com" // Production Render API
    : "https://webapi-8j7b.onrender.com";        // Local Development API

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(baseAddress)
});

// Add HTTP Client for authentication (if needed)
builder.Services.AddHttpClient("Auth", opt => opt.BaseAddress =
    new Uri("https://webapi-8j7b.onrender.com")) // Use the Render API URL for authentication as well
    .AddHttpMessageHandler<CutomHttpHandler>();

await builder.Build().RunAsync();