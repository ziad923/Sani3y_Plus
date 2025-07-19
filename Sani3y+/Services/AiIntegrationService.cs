using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public class AiIntegrationService: IAiIntegrationService
    {
        private readonly HttpClient _httpClient;

        public AiIntegrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetPredictedProfessionAsync(string userQuery)
        {
            var response = await _httpClient.PostAsJsonAsync("/query", new { query = userQuery });

            if (!response.IsSuccessStatusCode)
                throw new Exception("AI model request failed");

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            return json["detected_category"].ToString();
        }
    }
}
