using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TeslaAPIHandler.Services
{

    public class AuthTokenHandler : IAuthTokenHandler
    {
        private readonly HttpClient _httpClient;

        private const string refreshUrl = "https://auth.tesla.com/oauth2/v3/token";

        public AuthTokenHandler()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> RefreshTokenAsync(string refreshToken, string clientID)
        {
            var requestBody = new
            {
                grant_type = "refresh_token",
                client_id = clientID,
                refresh_token = refreshToken
            };

            string jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(refreshUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"[AUTH] Token refresh failed. Status: {response.StatusCode}, Reason: {response.ReasonPhrase}, Response: {errorDetails}");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("access_token", out JsonElement accessTokenElement))
            {
                throw new Exception($"[AUTH] Refresh token expired. Response: {jsonResponse}");
            }

            return accessTokenElement.GetString();
        }
    }

}