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
    class UserDefinedFunctions
    {
        public static async Task<Guid> GetOrCreateUserDefinedFunctionAsync(string name,
            Guid spaceId, Guid[] matchers, string script, HttpClient httpClient)
        {
            var udfs = await SearchUserDefinedFunctionAsync(name, httpClient);
            if (udfs.Any())
            {
                return udfs.First().Id;
            }
            var newId = await CreateUserDefinedFunctionAsync(name, spaceId, matchers,
                script, httpClient);
            return newId;
        }


        public static async Task<IEnumerable<UserDefinedFunctionQuery>> 
            SearchUserDefinedFunctionAsync(string name, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"userdefinedfunctions?$filter=Name eq '{name}'");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var udfs = JsonSerializer.Deserialize<IEnumerable<UserDefinedFunctionQuery>>(content,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return udfs;
            }
            return null;
        }
        public static async Task<Guid> CreateUserDefinedFunctionAsync(string name, 
            Guid spaceId, Guid[] matchers, string script, HttpClient httpClient)
        {
            var newUserDefinedFunction = new { name, spaceId, matchers };

            var content = JsonSerializer.Serialize(newUserDefinedFunction);

            var metadataContent = new StringContent(content, Encoding.UTF8, 
                MediaTypeNames.Application.Json);

            var multipartFormDataContent = new MultipartFormDataContent("userDefinedFunctionBoundary");
            multipartFormDataContent.Add(metadataContent, "metadata");
            multipartFormDataContent.Add(new StringContent(script), "contents");

            var response = await httpClient.PostAsync("userdefinedfunctions", multipartFormDataContent);
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
