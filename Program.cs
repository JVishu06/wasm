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

// Configure HttpClient for API communication
var apiUrl = builder.HostEnvironment.IsProduction()
    ? "https://webapi-8j7b.onrender.com" 
    : "http://localhost:5000"; 

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});

builder.Services.AddHttpClient("Auth", client => client.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<CutomHttpHandler>();

// Build and run the application
await builder.Build().RunAsync();
