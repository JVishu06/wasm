using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "2a4196ee6b0e779548ceaa014894a13b"; // Replace with your OpenWeatherMap API key

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Fetch weather data from OpenWeatherMap API
    public async Task<WeatherResponse?> GetWeatherAsync(string city)
    {
        var response = await _httpClient.GetFromJsonAsync<WeatherResponse>($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric");
        return response;
    }

    // Send weather data to Web API to store in MongoDB
    public async Task<bool> AddWeatherDataToApi(WeatherResponse weatherData)
    {
        var response = await _httpClient.PostAsJsonAsync("api/weather/add", weatherData);
        return response.IsSuccessStatusCode;
    }

    // Send request to add city to favorites
    public async Task<bool> AddCityToFavorites(string cityName)
    {
        var response = await _httpClient.PostAsJsonAsync("api/weather/favorites", cityName);
        return response.IsSuccessStatusCode;
    }

    // Get favorite cities from the backend API
    public async Task<List<string>> GetFavoriteCitiesFromApi()
    {
        var response = await _httpClient.GetFromJsonAsync<List<string>>("api/weather/favorites");
        return response ?? new List<string>();
    }
}

public class WeatherResponse
{
    public Main? Main { get; set; }
    public string? Name { get; set; }
}

public class Main
{
    public float? Temp { get; set; }
    public float? Pressure { get; set; }
    public float? Humidity { get; set; }
}
