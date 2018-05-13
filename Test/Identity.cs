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
using OpenAAP.Requests;
using FluentAssertions;

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
            var responseJson = JsonConvert.DeserializeObject<OpenAAP.Context.Identity>(responseString);

            responseJson.Id.Should().Be(Seeder.IdentitySingle.Id);
            responseJson.UserName.Should().Be(Seeder.IdentitySingle.UserName);
            responseJson.Email.Should().Be(Seeder.IdentitySingle.Email);
            responseJson.Description.Should().Be(Seeder.IdentitySingle.Description);
        }

        [Fact]
        public async Task GetUnknown()
        {
            var response = await _client.GetAsync($"/identity/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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

            var response = await _client.PostJsonAsync<OpenAAP.Context.Identity>($"/identity", create);

            response.Should().BeEquivalentTo(create);
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

            var createResponse = await _client.PostJsonAsync<OpenAAP.Context.Identity>($"/identity", create);
            createResponse.Should().BeEquivalentTo(create);

            var getResponse = await _client.GetJsonAsync<OpenAAP.Context.Identity>($"/identity/{createResponse.Id}");
            getResponse.Should().BeEquivalentTo(create);
            getResponse.Should().BeEquivalentTo(createResponse);
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

            var createResponse = await _client.PostJsonAsync<OpenAAP.Context.Identity>($"/identity", create);
            createResponse.Should().BeEquivalentTo(create);

            var getResponse = await _client.GetJsonAsync<OpenAAP.Context.Identity>($"/identity/{createResponse.Id}");
            getResponse.Should().BeEquivalentTo(create);
            getResponse.Should().BeEquivalentTo(createResponse);

            var deleteResponse = await _client.DeleteAsync($"/identity/{createResponse.Id}");
            deleteResponse.EnsureSuccessStatusCode();

            var getResponse2 = await _client.GetAsync($"/identity/{createResponse.Id}");
            getResponse2.StatusCode.Should().Be(HttpStatusCode.NotFound);
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

            var createResponse = await _client.PostJsonAsync<OpenAAP.Context.Identity>($"/identity", create);
            createResponse.Should().BeEquivalentTo(create);

            var getResponse = await _client.GetJsonAsync<OpenAAP.Context.Identity>($"/identity/{createResponse.Id}");
            getResponse.Should().BeEquivalentTo(create);
            getResponse.Should().BeEquivalentTo(createResponse);

            var update = new UpdateIdentityRequest
            {
                Description = "abcdx",
                UserName = "023164654",
            };

            var putResponse = await _client.PutJsonAsync<OpenAAP.Context.Identity>($"/identity/{createResponse.Id}", update);
            putResponse.Description.Should().Be(update.Description);
            putResponse.UserName.Should().Be(update.UserName);
            putResponse.Email.Should().Be(null);

            var getResponse2 = await _client.GetJsonAsync<OpenAAP.Context.Identity>($"/identity/{createResponse.Id}");
            getResponse2.Should().BeEquivalentTo(putResponse);
        }
    }
}
