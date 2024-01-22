using System.Text.Json;
using System.Web;

namespace BaseData
{
   internal class Program
   {
      static readonly HttpClient client = new HttpClient();

      static async Task Main()
      {
         Console.WriteLine("Városok és kerületek lekérdezése Magyarországon...");
         IEnumerable<string> cities = await GetCitiesAsync();
         foreach (string city in cities)
         {
            Console.WriteLine(city);
         }

         Console.WriteLine("Kérlek, add meg a város (kerület) nevét a utcanevek lekérdezéséhez:");
         string place = Console.ReadLine();

         if (string.IsNullOrEmpty(place))
         {
            Console.WriteLine("Nem adtál meg településnevet!");
            return;
         }

         var streetNames = await GetStreetsForPlace(place);
         streetNames.Distinct().OrderBy(x => x).ToList().ForEach(x => Console.WriteLine(x));
      }

      static IEnumerable<string> GetStreetNames(string responseBody)
      {
         using (JsonDocument document = JsonDocument.Parse(responseBody))
         {
            JsonElement root = document.RootElement;
            JsonElement elements = root.GetProperty("elements");

            foreach (JsonElement element in elements.EnumerateArray())
            {
               if (element.TryGetProperty("tags", out JsonElement tagsElement))
               {
                  if (tagsElement.TryGetProperty("name", out JsonElement nameElement))
                  {
                     yield return nameElement.GetString();
                  }
               }
            }
         }
      }

      static async Task<IEnumerable<string>> GetCitiesAsync()
      {
         string query = @"
            [out:json][timeout:25];
            area['name'='Magyarország']['boundary'='administrative'];
            node(area)['place'~'city|town'];
            out;";

         string url = "http://overpass-api.de/api/interpreter?data=" + HttpUtility.UrlEncode(query);
         HttpResponseMessage response = await client.GetAsync(url);
         response.EnsureSuccessStatusCode();
         string responseBody = await response.Content.ReadAsStringAsync();

         // Feldolgozás: városok nevének kinyerése
         List<string> cityNames = new List<string>();
         using (JsonDocument document = JsonDocument.Parse(responseBody))
         {
            JsonElement root = document.RootElement;
            JsonElement elements = root.GetProperty("elements");
            foreach (JsonElement element in elements.EnumerateArray())
            {
               if (element.TryGetProperty("tags", out JsonElement tagsElement))
               {
                  if (tagsElement.TryGetProperty("name", out JsonElement nameElement))
                  {
                     cityNames.Add(nameElement.GetString());
                  }
               }
            }
         }
         return cityNames;
      }

      static async Task<IEnumerable<string>> GetStreetsForPlace(string place)
      {
         // Ez a lekérdezés visszaadja a megadott város (vagy kerület) utcáit.
         string query = $@"
            [out:json][timeout:25];
            area['name'='{HttpUtility.UrlEncode(place)}']['boundary'='administrative'];
            (way(area)['highway']['name'];
            node(area)['highway']['name'];);
            (._;>;);
            out body;";

         string url = "http://overpass-api.de/api/interpreter?data=" + HttpUtility.UrlEncode(query);
         HttpResponseMessage response = await client.GetAsync(url);
         response.EnsureSuccessStatusCode();
         string responseBody = await response.Content.ReadAsStringAsync();

         return GetStreetNames(responseBody);
      }
   }
}
