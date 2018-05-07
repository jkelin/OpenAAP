using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAAP.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public static class Extensions
    {
        public static IWebHostBuilder CreateTestConfiguration(this IWebHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder();
            configuration.SetBasePath(Directory.GetCurrentDirectory());
            configuration.AddJsonFile("appsettings.json");
            configuration.AddEnvironmentVariables();

            var root = configuration.Build();

            return builder.UseConfiguration(root);
        }

        public static async Task<TOut> PostJsonAsync<TOut>(this HttpClient client, string url, object data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOut>(responseString);
        }

        public static async Task<TOut> PostJsonAsync<TOut>(this HttpClient client, string url, HttpStatusCode expectedStatusCode, object data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            Assert.Equal(expectedStatusCode, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOut>(responseString);
        }

        public static async Task<TOut> GetJsonAsync<TOut>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOut>(responseString);
        }

        public static async Task<TOut> GetJsonAsync<TOut>(this HttpClient client, string url, HttpStatusCode expectedStatusCode)
        {
            var response = await client.GetAsync(url);

            Assert.Equal(expectedStatusCode, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOut>(responseString);
        }

        public static async Task<TOut> PutJsonAsync<TOut>(this HttpClient client, string url, object data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOut>(responseString);
        }

        public static async Task<TOut> PutJsonAsync<TOut>(this HttpClient client, string url, HttpStatusCode expectedStatusCode, object data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, content);

            Assert.Equal(expectedStatusCode, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOut>(responseString);
        }
    }
}
