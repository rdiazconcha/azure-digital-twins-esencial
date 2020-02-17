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
    class Matchers
    {
        public static async Task<Guid> GetOrCreateMatcherAsync(string name, Guid spaceId,
            string dataTypeValue, HttpClient httpClient)
        {
            var devices = await SearchMatcherAsync(name, httpClient);
            if (devices.Any())
            {
                return devices.First().Id;
            }
            var newId = await CreateMatcherAsync(name, spaceId, dataTypeValue, httpClient);
            return newId;
        }
        public static async Task<IEnumerable<MatcherQuery>> SearchMatcherAsync(string name, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"matchers?$filter=Name eq '{name}'");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var matchers = JsonSerializer.Deserialize<IEnumerable<MatcherQuery>>(content,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return matchers;
            }
            return null;
        }
        public static async Task<Guid> CreateMatcherAsync(string name,
            Guid spaceId, string dataTypeValue, HttpClient httpClient)
        {
            var newCondition = new
            {
                target = "Sensor",
                path = "$.dataType",
                value = $"\"{dataTypeValue}\"",
                comparison = "Equals"
            };
            var newMatcher = new { name, spaceId, conditions = new[] { newCondition } };
            var content = JsonSerializer.Serialize(newMatcher);
            var response = await httpClient.PostAsync("matchers",
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
