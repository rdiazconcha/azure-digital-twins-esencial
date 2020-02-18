using Manager.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Manager.Api
{
    class Tools
    {
        public static async Task<IEnumerable<T>> GetAllAsync<T>(string endpoint, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var devices = JsonSerializer.Deserialize<IEnumerable<T>>(content,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return devices;
            }
            return null;
        }

        public static async Task<bool> DeleteAsync(string endpoint, Guid id, HttpClient httpClient)
        {
            var response = await httpClient.DeleteAsync($"{endpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        public static async Task UnprovisionAsync(HttpClient httpClient)
        {
            var sensors = await Tools.GetAllAsync<SensorQuery>("sensors", httpClient);
            foreach (var item in sensors)
            {
                var deleted = await Tools.DeleteAsync("sensors", item.Id, httpClient);
                Console.WriteLine(deleted ? "Sensor borrado" : "No se pudo borrar el sensor");
            }

            var devices = await Tools.GetAllAsync<DeviceQuery>("devices", httpClient);
            foreach (var item in devices)
            {
                var deleted = await Tools.DeleteAsync("devices", item.Id, httpClient);
                Console.WriteLine(deleted ? "Dispositivo borrado" : "No se pudo borrar el dispositivo");
            }

            var resources = await Tools.GetAllAsync<ResourceQuery>("resources", httpClient);
            foreach (var item in resources)
            {
                var deleted = await Tools.DeleteAsync("resources", item.Id, httpClient);
                Console.WriteLine(deleted ? "Recurso borrado" : "No se pudo borrar el recurso");
            }

            var udfs = await Tools.GetAllAsync<UserDefinedFunctionQuery>("userdefinedfunctions", httpClient);
            foreach (var item in udfs)
            {
                var deleted = await Tools.DeleteAsync("userdefinedfunctions", item.Id, httpClient);
                Console.WriteLine(deleted ? "Udf borrado" : "No se pudo borrar el Udf");
            }

            var matchers = await Tools.GetAllAsync<MatcherQuery>("matchers", httpClient);
            foreach (var item in matchers)
            {
                var deleted = await Tools.DeleteAsync("matchers", item.Id, httpClient);
                Console.WriteLine(deleted ? "Matcher borrado" : "No se pudo borrar el matcher");
            }

            var spaces = await Tools.GetAllAsync<SpaceQuery>("spaces", httpClient);
            foreach (var item in spaces)
            {
                var deleted = await Tools.DeleteAsync("spaces", item.Id, httpClient);
                Console.WriteLine(deleted ? "Espacio borrado" : "No se pudo borrar el espacio");
            }
        }
    }
}