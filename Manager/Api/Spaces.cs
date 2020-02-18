using Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Manager.Api
{
    class Spaces
    {
        public static async Task<IEnumerable<SpaceQuery>> GetAllSpacesAsync(HttpClient httpClient, 
            bool includeValues = false)
        {
            var uri = includeValues ? $"spaces?includes=values" : "spaces";
            var response = await httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var spaces = JsonSerializer.Deserialize<IEnumerable<SpaceQuery>>(content, 
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return spaces;
            }
            return null;
        }
        public static async Task<Guid> GetOrCreateSpaceAsync(string name, string type, HttpClient httpClient)
        {
            var spaces = await SearchSpaceAsync(name, httpClient);
            if (spaces.Any())
            {
                return spaces.First().Id;
            }
            var newId = await CreateSpaceAsync(name, type, httpClient);
            return newId;
        }

        private static async Task<IEnumerable<SpaceQuery>> SearchSpaceAsync(string spaceName, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"spaces?$filter=Name eq '{spaceName}'");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var spaces = JsonSerializer.Deserialize<IEnumerable<SpaceQuery>>(content,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return spaces;
            }
            return null;
        }
        private static async Task<Guid> CreateSpaceAsync(string name, string type, HttpClient httpClient)
        {
            var newSpace = new { name, type };
            var content = JsonSerializer.Serialize(newSpace);
            var response = await httpClient.PostAsync("spaces",
                new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var newId = JsonSerializer.Deserialize<Guid>(result);
                return newId;
            }
            return Guid.Empty;
        }
    }
}
