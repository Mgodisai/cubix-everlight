using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace LocationDataLib
{
    public class LocationService
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;

        public LocationService(HttpClient client, IMemoryCache cache)
        {
            _client = client;
            _cache = cache;
        }

        public async Task<IEnumerable<string>> GetCitiesAsync()
        {
            return await _cache.GetOrCreateAsync("CitiesCacheKey", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return await FetchCitiesFromApi();
            });
        }

        public async Task<IEnumerable<string>> GetStreetsForPlace(string place)
        {
            string cacheKey = $"Streets_{place}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return await FetchStreetsFromApi(place);
            });
        }

        private async Task<IEnumerable<string>> FetchCitiesFromApi()
        {
            string query = @"
                [out:json][timeout:25];
                area['name'='Magyarország']['boundary'='administrative']->.hungary;
                (
                  node(area.hungary)['place'='city'];
                  node(area.hungary)['place'='town'];
                  node(area.hungary)['place'='village'];
                  node(area.hungary)['place'='suburb'];
                );
                out;";

            string url = "http://overpass-api.de/api/interpreter?data=" + System.Web.HttpUtility.UrlEncode(query);
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return ParseCityNames(responseBody);
        }

        private async Task<IEnumerable<string>> FetchStreetsFromApi(string place)
        {
            string query = $@"
                [out:json][timeout:25];
                {{{{geocodeArea:{place}}}}}->.searchArea
                    (
                      way[""highway""][""name""](area.searchArea);
                    );
                for (t[""name""])
                {{
                  make street name=_.val;
                  out;
                }}";

            string url = "http://overpass-api.de/api/interpreter?data=" + System.Web.HttpUtility.UrlEncode(query);
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return ParseStreetNames(responseBody).OrderBy(s=>s);
        }

        private IEnumerable<string> ParseCityNames(string responseBody)
        {
            using JsonDocument document = JsonDocument.Parse(responseBody);
            JsonElement root = document.RootElement;
            JsonElement elements = root.GetProperty("elements");
            var cities = new List<string>();

            foreach (JsonElement element in elements.EnumerateArray())
            {
                if (element.TryGetProperty("tags", out JsonElement tagsElement) &&
                    tagsElement.TryGetProperty("name", out JsonElement nameElement))
                {
                    cities.Add(nameElement.GetString());
                }
            }

            return cities.Distinct();
        }

        private IEnumerable<string> ParseStreetNames(string responseBody)
        {
            using JsonDocument document = JsonDocument.Parse(responseBody);
            JsonElement root = document.RootElement;
            JsonElement elements = root.GetProperty("elements");
            var streets = new List<string>();

            foreach (JsonElement element in elements.EnumerateArray())
            {
                if (element.TryGetProperty("tags", out JsonElement tagsElement) &&
                    tagsElement.TryGetProperty("name", out JsonElement nameElement))
                {
                    streets.Add(nameElement.GetString());
                }
            }

            return streets.Distinct();
        }
    }
}
