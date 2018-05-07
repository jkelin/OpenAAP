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
    public class Registration : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public Registration()
        {
            _server = new TestServer(new WebHostBuilder().CreateTestConfiguration().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        [Theory]
        [InlineData("65465456")]
        [InlineData(";sjdflksdjf3279847alksj hdlkjahs dashdlkjahskl jdhakjs dhkaljsdhkljas hd981729aslkjdn*//*")]
        [InlineData("123123")]
        [InlineData("ěščřžýáíéóň")]
        public async Task Register(string password)
        {
            var regData = new PasswordRegisterRequest
            {
                Password = password
            };

            var regResp = await _client.PostJsonAsync<SessionModel>(
                $"/identity/{Seeder.IdentityNone.Id}/password/register",
                regData
            );

            var loginData = new PasswordLoginRequest
            {
                Password = password
            };

            var loginResp = await _client.PostJsonAsync<SessionModel>(
                $"/identity/{Seeder.IdentityNone.Id}/password/login",
                loginData
            );
        }
    }
}
