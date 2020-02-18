using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Manager.Api
{
    class RoleAssignments
    {
        public static async Task<Guid> CreateRoleAssignmentAsync(Guid objectId,
            Guid spaceId, HttpClient httpClient)
        {
            var newRoleAssignment = new
            {
                roleId = "98e44ad7-28d4-4007-853b-b9968ad132d1",
                objectId,
                path = $"/{spaceId}",
                objectIdType = "UserDefinedFunctionId"
            };
            var content = JsonSerializer.Serialize(newRoleAssignment);
            var response = await httpClient.PostAsync("roleassignments",
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
