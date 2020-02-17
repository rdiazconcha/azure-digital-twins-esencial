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
    class Sensors
    {
        public static async Task<Guid> GetOrCreateSensorAsync(string dataType, 
            string hardwareId, Guid deviceId, HttpClient httpClient)
        {
            var sensors = await SearchSensorAsync(hardwareId, httpClient);
            if (sensors.Any())
            {
                return sensors.First().Id;
            }
            var newId = await CreateSensorAsync(dataType, hardwareId, deviceId, httpClient);
            return newId;
        }
        public static async Task<IEnumerable<SensorQuery>> SearchSensorAsync(string hardwareId,
            HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"sensors?$filter=HardwareId eq '{hardwareId}'");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sensors = JsonSerializer.Deserialize<IEnumerable<SensorQuery>>(content,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return sensors;
            }
            return null;
        }
        public static async Task<Guid> CreateSensorAsync(string dataType,
            string hardwareId,
            Guid deviceId, HttpClient httpClient)
        {
            var newSensor = new { dataType, hardwareId, deviceId };
            var content = JsonSerializer.Serialize(newSensor);
            var response = await httpClient.PostAsync("sensors",
                new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var newId = JsonSerializer.Deserialize<Guid>(result);
            }
            return Guid.Empty;
        }
    }
}
