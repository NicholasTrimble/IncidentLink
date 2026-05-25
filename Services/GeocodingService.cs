using System.Text.Json;

namespace IncidentLink.Services
{
    public class GeocodingService
    {
        private readonly HttpClient _http;

        public GeocodingService(HttpClient http)
        {
            _http = http;
        }

        public async Task<(double lat, double lng)?> GeocodeAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}";

            _http.DefaultRequestHeaders.UserAgent.ParseAdd("IncidentLinkApp/1.0");

            var json = await _http.GetStringAsync(url);

            var doc = JsonDocument.Parse(json);
            var first = doc.RootElement.EnumerateArray().FirstOrDefault();

            if (first.ValueKind == JsonValueKind.Undefined)
                return null;

            var lat = double.Parse(first.GetProperty("lat").GetString());
            var lon = double.Parse(first.GetProperty("lon").GetString());

            return (lat, lon);
        }
    }
}