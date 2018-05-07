using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAAP;
using OpenAAP.Context;
using OpenAAP.Options;
using OpenAAP.Requests;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.PasswordAuthentication
{
    public class Delete : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public Delete()
        {
            _server = new TestServer(new WebHostBuilder().CreateTestConfiguration().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        [Fact]
        public async Task CannotLogin()
        {
            // Confirm login works before deletion
            await _client.PostJsonAsync<SessionModel>(
                $"/identity/{Seeder.IdentitySingle.Id}/password/login",
                new PasswordLoginRequest { Password = "xyz" }
            );

            // Delete auth
            var deleteResp = await _client.DeleteAsync(
                $"/identity/{Seeder.IdentitySingle.Id}/password"
            );
            deleteResp.EnsureSuccessStatusCode();

            // Confirm login does not work after deletion
            await _client.PostJsonAsync<SessionModel>(
                $"/identity/{Seeder.IdentitySingle.Id}/password/login",
                HttpStatusCode.Unauthorized,
                new PasswordLoginRequest { Password = "xyz" }
            );
        }

        [Fact]
        public async Task SessionExpired()
        {
            var identity = Seeder.IdentityNormal.Id;

            // Confirm login works before deletion
            var oldSession = await _client.PostJsonAsync<SessionModel>(
                $"/identity/{identity}/password/login",
                new PasswordLoginRequest { Password = "quick brown fox" }
            );

            // Confirm that session works
            await _client.GetJsonAsync<SessionModel>(
                $"/session/{oldSession.SessionId}"
            );

            // Delete auth
            var deleteResp = await _client.DeleteAsync(
                $"/identity/{identity}/password"
            );
            deleteResp.EnsureSuccessStatusCode();

            // Session no longer works
            await _client.GetJsonAsync<SessionModel>(
                $"/session/{oldSession.SessionId}",
                HttpStatusCode.Unauthorized
            );
        }
    }
}
