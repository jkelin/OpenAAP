using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAAP.Context;
using OpenAAP.Errors;
using OpenAAP.Options;
using OpenAAP.Requests;
using OpenAAP.Services.PasswordHashing;
using OpenAAP.Services.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Controllers
{
    [Route("identity/{identityId}/password")]
    [ProducesResponseType(404, Type = typeof(IdentityNotFound))]
    public class PasswordAuthenticationController : Controller
    {
        private readonly OpenAAPContext ctx;
        private readonly ISessionStorageService sessionStorage;
        private readonly PasswordHashingService hasher;
        private readonly HashingOptions hashingOptions;
        private readonly SessionOptions sessionOptions;

        public PasswordAuthenticationController(
            OpenAAPContext context,
            ISessionStorageService sessionStorage,
            PasswordHashingService hasher,
            IOptions<HashingOptions> hashingOptions,
            IOptions<SessionOptions> sessionOptions
        )
        {
            ctx = context;
            this.sessionStorage = sessionStorage;
            this.hasher = hasher;
            this.hashingOptions = hashingOptions.Value;
            this.sessionOptions = sessionOptions.Value;
        }

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(Session))]
        [ProducesResponseType(404, Type = typeof(NoPasswordAuthenticationFound))]
        [ProducesResponseType(404, Type = typeof(ActivePasswordAuthenticationNotFound))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login(Guid identityId, [FromBody]PasswordLoginRequest req)
        {
            var auths = ctx.PasswordAuthentication.Where(x => x.IdentityId == identityId);

            if (!await auths.AnyAsync())
            {
                return NotFound(new PasswordInvalid());
            }

            var enabledAuths = auths.Where(x => x.DisabledAt == null);

            if (!await auths.AnyAsync())
            {
                return NotFound(new ActivePasswordAuthenticationNotFound());
            }

            foreach (var auth in enabledAuths)
            {
                if (await PasswordMatches(req.Password, auth))
                {
                    var session = await CreateSession(identityId);
                    return Ok(session);
                }
            }

            // TODO if password matches one of the disabled passwords, return error that says so

            return Unauthorized();
        }

        async Task<Session> CreateSession(Guid identityId)
        {
            var session = new Session
            {
                Id = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddMilliseconds(sessionOptions.SessionExpirationMs),
                IdentityId = identityId
            };

            await sessionStorage.StoreSession(session);

            return session;
        }

        async Task<bool> PasswordMatches(string password, PasswordAuthentication auth)
        {
            var passwordBytes = password.ToBytes();

            var hash = await hasher.Hash(passwordBytes, auth.Salt, auth.Algorithm);
            if (!auth.Hash.ArrayEquals(hash))
            {
                return false;
            }

            // TODO automatically upgrade algorithm here if it's outdated

            return true;
        }

        [HttpDelete]
        public async Task<IActionResult> Unregister(Guid identityId)
        {
            await DisableRegistration(identityId);
            await ctx.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(Session))]
        public async Task<IActionResult> Register(Guid identityId, [FromBody]PasswordRegisterRequest req)
        {
            var algo = hashingOptions.TargetAlgorithm;
            var salt = await hasher.GenerateSalt(hashingOptions.SaltLength);
            var pwHash = await hasher.Hash(req.Password.ToBytes(), salt, algo);

            var auth = new PasswordAuthentication
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Algorithm = algo,
                Hash = pwHash,
                Salt = salt,
                IdentityId = identityId,
            };

            await DisableRegistration(identityId);
            await ctx.PasswordAuthentication.AddAsync(auth);
            await ctx.SaveChangesAsync();

            var session = await CreateSession(identityId);

            return Ok(session);
        }

        private async Task DisableRegistration(Guid identityId)
        {
            var registrations = await ctx.PasswordAuthentication.Where(x => x.IdentityId == identityId && x.DisabledAt == null).ToListAsync();
            foreach (var reg in registrations)
            {
                reg.DisabledAt = DateTime.UtcNow;
            }

            await sessionStorage.DeleteSessionByIdentityId(identityId);
        }
    }
}
