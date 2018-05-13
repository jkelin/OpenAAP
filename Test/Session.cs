using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using OpenAAP;
using OpenAAP.Context;
using OpenAAP.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public class Session : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public Session()
        {
            _server = new TestServer(new WebHostBuilder().CreateTestConfiguration().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        [Fact]
        public async Task NotExpired()
        {
            var login = await _client.PostJsonAsync<OpenAAP.Context.Session>(
                $"/identity/{Seeder.IdentitySingle.Id}/password/login",
                new PasswordLoginRequest
                {
                    Password = "xyz"
                }
            );

            var session = await _client.GetJsonAsync<OpenAAP.Context.Session>(
                $"/session/{login.Id}"
            );

            login.Id.Should().Be(session.Id);
            login.IdentityId.Should().Be(session.IdentityId);
            login.IdentityId.Should().Be(Seeder.IdentitySingle.Id);
        }

        [Fact]
        public async Task Expired()
        {
            var login = await _client.PostJsonAsync<OpenAAP.Context.Session>(
                $"/identity/{Seeder.IdentitySingle.Id}/password/login",
                new PasswordLoginRequest
                {
                    Password = "xyz"
                }
            );

            await Task.Delay(600);

            var session = await _client.GetJsonAsync<OpenAAP.Context.Session>(
                $"/session/{login.Id}",
                System.Net.HttpStatusCode.Unauthorized
            );
        }

        [Fact]
        public async Task Multiple()
        {
            var login = await _client.PostJsonAsync<OpenAAP.Context.Session>(
                $"/identity/{Seeder.IdentitySingle.Id}/password/login",
                new PasswordLoginRequest
                {
                    Password = "xyz"
                }
            );

            var login2 = await _client.PostJsonAsync<OpenAAP.Context.Session>(
                $"/identity/{Seeder.IdentitySingle.Id}/password/login",
                new PasswordLoginRequest
                {
                    Password = "xyz"
                }
            );

            var session = await _client.GetJsonAsync<OpenAAP.Context.Session>(
                $"/session/{login.Id}"
            );

            var session2 = await _client.GetJsonAsync<OpenAAP.Context.Session>(
                $"/session/{login2.Id}"
            );

            login.Id.Should().Be(session.Id);
            login.IdentityId.Should().Be(session.IdentityId);
            login.IdentityId.Should().Be(Seeder.IdentitySingle.Id);

            login2.Id.Should().Be(session2.Id);
            login2.IdentityId.Should().Be(session2.IdentityId);
            login2.IdentityId.Should().Be(Seeder.IdentitySingle.Id);
        }
    }
}
