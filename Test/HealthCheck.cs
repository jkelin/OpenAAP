using Microsoft.AspNetCore.Hosting;
using OpenAAP;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using OpenAAP.Requests;
using Newtonsoft.Json;
using System.Text;
using FluentAssertions;

namespace Test
{
    public class HealthCheck
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public HealthCheck()
        {
            _server = new TestServer(new WebHostBuilder().CreateTestConfiguration().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Ping()
        {
            var response = await _client.GetAsync("/health/ping");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().Be("pong");
        }

        [Fact]
        public async Task Unknown()
        {
            var response = await _client.GetAsync("/health/aslkdfjslkdjf");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("sdfsdf", null, "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b", null)]
        [InlineData("sdfsdfghjghjghj234234234234", null, "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b", null)]
        [InlineData("sdfsdf", null, "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b", "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b")]
        [InlineData("sdfsd", null, "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b", "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b")]
        [InlineData("sdfsd", "test@example.com", "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b", "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b")]
        public async Task BodyValidatorValid(string username = null, string email = null, string id1 = null, string id2 = null)
        {
            var data = new
            {
                RequiredUsername = username,
                OptionalEmail = email,
                RequiredId1 = id1,
                OptionalId2 = id2
            };

            var response = await _client.PostJsonAsync<ValidatorRequest>("/health/body-validator", data);
            response.RequiredUsername.Should().Be(username);
            response.OptionalEmail.Should().Be(email);
            response.RequiredId1?.Should().Be(id1);
            response.OptionalId2?.Should().Be(id2);
        }

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData("dfgdfg", null, null, null)]
        [InlineData("dfgdfg", null, null, "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b")]
        [InlineData("dfgdfg", null, "e81ae4f8-3f7d-4d54-97b8", "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b")]
        [InlineData("dfg", null, "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b", "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b")]
        [InlineData("dfgfgh", null, "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b", "e81ae4f8-3f7d-4d54-97b8")]
        [InlineData("dfgfgh", "asdasd", "e81ae4f8-3f7d-4d54-97b8-0cc12f47223b", null)]
        public async Task BodyValidatorInvalid(string username = null, string email = null, string id1 = null, string id2 = null)
        {
            var data = new
            {
                RequiredUsername = username,
                OptionalEmail = email,
                RequiredId1 = id1,
                OptionalId2 = id2
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/health/body-validator", content);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("dfdfgdfg")]
        [InlineData("{asdasd")]
        [InlineData("")]
        public async Task BodyValidatorInvalidBadBody(string data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/health/body-validator", content);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
