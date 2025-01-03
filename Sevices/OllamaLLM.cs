
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OllamaChatbot.Services
{
    public class OllamaLLM
    {
        private readonly string _model;
        private readonly HttpClient _httpClient;
        private string _customPrompt = string.Empty; // Added field for custom prompt

        public OllamaLLM(string model)
        {
            // Ensure model is not null or empty
            _model = string.IsNullOrWhiteSpace(model)
                ? throw new ArgumentException("Model cannot be null or empty.", nameof(model))
                : model;

            _httpClient = new HttpClient();
        }

        // Method to set a custom prompt (fine-tuning the prompt for each request)
        public void SetCustomPrompt(string customPrompt)
        {
            if (string.IsNullOrWhiteSpace(customPrompt))
            {
                throw new ArgumentException("Custom prompt cannot be null or empty.", nameof(customPrompt));
            }
            _customPrompt = customPrompt; // Store the custom prompt
        }

        public async Task<string> InvokeAsync(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));
            }

            string prompt = string.IsNullOrEmpty(_customPrompt) ? input : $"{_customPrompt} {input}";

            var requestContent = new
            {
                messages = new[]
                {
            new { role = "user", content = prompt }
        },
                model = _model
            };

            var apiKey = "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"; // Use secure storage for your API key
            var requestJson = JsonSerializer.Serialize(requestContent);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
            {
                Content = new StringContent(requestJson)
            };

            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonDocument>(responseContent);

                if (jsonResponse != null && jsonResponse.RootElement.TryGetProperty("choices", out var choices))
                {
                    var message = choices[0].GetProperty("message").GetProperty("content").GetString();
                    return message ?? "No response from the model.";
                }

                return "Unexpected response format from Groq API.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error invoking Groq API: {ex.Message}");
                return $"Error invoking Groq API: {ex.Message}";
            }
        }

    }
}
