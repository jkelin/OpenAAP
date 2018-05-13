using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using OpenAAP;
using OpenAAP.Context;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.PasswordAuthentication
{
    public class Login : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public Login()
        {
            _server = new TestServer(new WebHostBuilder().CreateTestConfiguration().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        [Fact]
        public async Task InvalidIdentity()
        {
            var response = await _client.PostAsync(
                $"/identity/1d8b44ff-9cb7-42f7-bc3b-5c3d73400ca1/password/login",
                 new StringContent(JsonConvert.SerializeObject(new { password = "sdfsdfsdf" }), Encoding.UTF8, "application/json")
            );
            HttpStatusCode.NotFound.Should().Be(response.StatusCode);
        }

        [Fact]
        public async Task TestLogin()
        {
            var response = await _client.PostJsonAsync<OpenAAP.Context.Session>(
                $"/identity/{Seeder.IdentitySingle.Id}/password/login",
                new { password = "xyz" }
            );

            response.IdentityId.Should().Be(Seeder.IdentitySingle.Id);
        }

        [Fact]
        public async Task LoginInvalidPassword()
        {
            var response = await _client.PostAsync(
                $"/identity/{Seeder.IdentitySingle.Id}/password/login",
                 new StringContent(JsonConvert.SerializeObject(new { password = "sdfsdfsdf" }), Encoding.UTF8, "application/json")
            );
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task LoginOldPassword()
        {
            var response = await _client.PostAsync(
                $"/identity/{Seeder.IdentityNormal.Id}/password/login",
                 new StringContent(JsonConvert.SerializeObject(new { password = "abcd" }), Encoding.UTF8, "application/json")
            );
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var response2 = await _client.PostAsync(
                $"/identity/{Seeder.IdentityNormal.Id}/password/login",
                 new StringContent(JsonConvert.SerializeObject(new { password = "quick brown fox" }), Encoding.UTF8, "application/json")
            );
            response2.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
