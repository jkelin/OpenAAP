using Microsoft.AspNetCore.Hosting;
using OpenAAP;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using OpenAAP.Context;
using Newtonsoft.Json;
using System.Text;
using DeepEqual.Syntax;
using OpenAAP.Requests;

namespace Test
{
    public class Identity
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public Identity()
        {
            _server = new TestServer(new WebHostBuilder().CreateTestConfiguration().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GetPregenerated()
        {
            var response = await _client.GetAsync($"/identity/{Seeder.IdentitySingle.Id}");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<IdentityModel>(responseString);

            responseJson.ShouldDeepEqual(Seeder.IdentitySingle);
        }

        [Fact]
        public async Task GetUnknown()
        {
            var response = await _client.GetAsync($"/identity/{Guid.NewGuid()}");
            Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create()
        {
            var create = new CreateIdentityRequest
            {
                Description = "abcd",
                Email = "asdasd@asdads.com",
                UserName = "sdfsdfsdf",
            };

            var response = await _client.PostJsonAsync<IdentityModel>($"/identity", create);

            response.WithDeepEqual(create).IgnoreUnmatchedProperties().Assert();
        }

        [Fact]
        public async Task CreateGet()
        {
            var create = new CreateIdentityRequest
            {
                Description = "abcd",
                Email = "asdasd@asdads.com",
                UserName = "sdfsdfsdf",
            };

            var createResponse = await _client.PostJsonAsync<IdentityModel>($"/identity", create);
            createResponse.WithDeepEqual(create).IgnoreUnmatchedProperties().Assert();

            var getResponse = await _client.GetJsonAsync<IdentityModel>($"/identity/{createResponse.Id}");
            getResponse.WithDeepEqual(create).IgnoreUnmatchedProperties().Assert();
            getResponse.WithDeepEqual(createResponse).IgnoreUnmatchedProperties().Assert();
        }

        [Fact]
        public async Task CreateDeleteGet()
        {
            var create = new CreateIdentityRequest
            {
                Description = "abcd",
                Email = "asdasd@asdads.com",
                UserName = "sdfsdfsdf",
            };

            var createResponse = await _client.PostJsonAsync<IdentityModel>($"/identity", create);
            createResponse.WithDeepEqual(create).IgnoreUnmatchedProperties().Assert();

            var getResponse = await _client.GetJsonAsync<IdentityModel>($"/identity/{createResponse.Id}");
            getResponse.WithDeepEqual(create).IgnoreUnmatchedProperties().Assert();
            getResponse.WithDeepEqual(createResponse).IgnoreUnmatchedProperties().Assert();

            var deleteResponse = await _client.DeleteAsync($"/identity/{createResponse.Id}");
            deleteResponse.EnsureSuccessStatusCode();

            var getResponse2 = await _client.GetAsync($"/identity/{createResponse.Id}");
            Assert.Equal(getResponse2.StatusCode, HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task CreateGetUpdate()
        {
            var create = new CreateIdentityRequest
            {
                Description = "abcd",
                Email = "asdasd@asdads.com",
                UserName = "sdfsdfsdf",
            };

            var createResponse = await _client.PostJsonAsync<IdentityModel>($"/identity", create);
            createResponse.WithDeepEqual(create).IgnoreUnmatchedProperties().Assert();

            var getResponse = await _client.GetJsonAsync<IdentityModel>($"/identity/{createResponse.Id}");
            getResponse.WithDeepEqual(create).IgnoreUnmatchedProperties().Assert();
            getResponse.WithDeepEqual(createResponse).IgnoreUnmatchedProperties().Assert();

            var update = new UpdateIdentityRequest
            {
                Description = "abcdx",
                UserName = "023164654",
            };

            var putResponse = await _client.PutJsonAsync<IdentityModel>($"/identity/{createResponse.Id}", update);
            Assert.Equal(putResponse.Description, update.Description);
            Assert.Equal(putResponse.UserName, update.UserName);
            Assert.Equal(putResponse.Email, null);

            var getResponse2 = await _client.GetJsonAsync<IdentityModel>($"/identity/{createResponse.Id}");
            getResponse2.WithDeepEqual(putResponse).IgnoreUnmatchedProperties().Assert();
        }
    }
}
