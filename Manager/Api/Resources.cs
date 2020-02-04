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
    class Resources
    {
        public static async Task<Guid> GetOrCreateResourceAsync(Guid spaceId, string type, HttpClient httpClient)
        {
            var resources = await SearchResourceAsync(type, httpClient);
            if (resources.Any())
            {
                return resources.First().Id;
            }
            var newId = await CreateResourceAsync(spaceId, type, httpClient);
            return newId;
        }

        public static async Task<IEnumerable<ResourceQuery>> SearchResourceAsync(string type, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"resources?$filter=Type eq '{type}'");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var resources = JsonSerializer.Deserialize<IEnumerable<ResourceQuery>>(content,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return resources;
            }
            return null;
        }
        public static async Task<ResourceQuery> GetResourceAsync(Guid resourceId, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"resources/{resourceId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ResourceQuery>(content,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return result;
            }
            return null;
        }
        public static async Task<Guid> CreateResourceAsync(Guid spaceId, string type, HttpClient httpClient)
        {
            var newResource = new { spaceId, type };
            var content = JsonSerializer.Serialize(newResource);
            var response = await httpClient.PostAsync("resources", 
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