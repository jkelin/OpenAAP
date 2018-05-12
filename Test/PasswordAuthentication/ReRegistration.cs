using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using OpenAAP;
using OpenAAP.Context;
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
    public class ReRegistration : IAsyncLifetime
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private readonly string password1 = "abcd123";
        private readonly string password2 = "jksfdhsdkjf";

        private OpenAAP.Context.Session session1;
        private OpenAAP.Context.Session session2;

        public ReRegistration()
        {
            _server = new TestServer(new WebHostBuilder().CreateTestConfiguration().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        public async Task InitializeAsync()
        {
            session1 = await Register(password1);
            session2 = await Register(password2);
        }

        public Task DisposeAsync()
        {
            _server.Dispose();
            return Task.CompletedTask;
        }

        public async Task<OpenAAP.Context.Session> Register(string password)
        {
            var data = new PasswordRegisterRequest
            {
                Password = password
            };

            return await _client.PostJsonAsync<OpenAAP.Context.Session>(
                $"/identity/{Seeder.IdentityNone.Id}/password/register",
                data
            );
        }

        [Fact]
        public async Task OldPasswordDoesNotWorkAnymore()
        {
            var reg = await _client.PostJsonAsync<OpenAAP.Context.Session>(
                $"/identity/{Seeder.IdentityNone.Id}/password/login",
                HttpStatusCode.Unauthorized,
                new PasswordLoginRequest
                {
                    Password = password1
                }
            );
        }

        [Fact]
        public async Task Login()
        {
            var reg = await _client.PostJsonAsync<OpenAAP.Context.Session>(
                $"/identity/{Seeder.IdentityNone.Id}/password/login",
                new PasswordLoginRequest
                {
                    Password = password2
                }
            );
        }

        [Fact]
        public async Task OldSessionIsExpired()
        {
            var reg = await _client.GetJsonAsync<OpenAAP.Context.Session>(
                $"/session/{session1.Id}",
                HttpStatusCode.Unauthorized
            );
        }

        [Fact]
        public async Task NewSessionIsActive()
        {
            var reg = await _client.GetJsonAsync<OpenAAP.Context.Session>(
                $"/session/{session2.Id}"
            );
        }
    }
}
