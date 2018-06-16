using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAAP.Context;
using OpenAAP.Errors;
using OpenAAP.Options;
using OpenAAP.Requests;
using OpenAAP.Responses;
using OpenAAP.Services.PasswordHashing;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Controllers
{
    [Route("identity/{identityId}/password")]
    [ProducesResponseType(404, Type = typeof(IdentityNotFound))]
    public class PasswordAuthenticationController : Controller
    {
        private readonly OpenAAPContext ctx;
        private readonly PasswordHashingService hasher;
        private readonly IOptions<HashingOptions> hashingOptions;

        public PasswordAuthenticationController(
            OpenAAPContext context,
            PasswordHashingService hasher,
            IOptions<HashingOptions> hashingOptions
        )
        {
            ctx = context;
            this.hasher = hasher;
            this.hashingOptions = hashingOptions;
        }

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(AuthenticationResponse))]
        [ProducesResponseType(404, Type = typeof(NoPasswordAuthenticationFound))]
        [ProducesResponseType(404, Type = typeof(ActivePasswordAuthenticationNotFound))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login(Guid identityId, [FromBody]PasswordLoginRequest req)
        {
            var auths = ctx.PasswordAuthentications.Where(x => x.IdentityId == identityId);

            if (!await auths.AnyAsync())
            {
                return NotFound(new PasswordInvalid());
            }

            var enabledAuths = auths.Where(x => x.DisabledAt == null);

            if (!await auths.AnyAsync())
            {
                return NotFound(new ActivePasswordAuthenticationNotFound());
            }

            foreach (var auth in enabledAuths.ToArray())
            {
                if (await PasswordMatches(req.Password, auth))
                {
                    return Ok(new AuthenticationResponse { IdentityId = auth.IdentityId });
                }
            }

            // TODO if password matches one of the disabled passwords, return error that says so

            return Unauthorized();
        }

        async Task<bool> PasswordMatches(string password, PasswordAuthentication auth)
        {
            var data = StoredPassword.Deserialize(auth.EncodedStoredPassword);

            var passwordBytes = password.ToBytes();

            var hash = await hasher.Hash(passwordBytes, data.Salt, data.Options);
            if (!data.Hash.ArrayEquals(hash))
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
        [ProducesResponseType(200, Type = typeof(AuthenticationResponse))]
        public async Task<IActionResult> Register(Guid identityId, [FromBody]PasswordRegisterRequest req)
        {
            var hash = await HashPassword(req.Password);

            var auth = new PasswordAuthentication
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                EncodedStoredPassword = hash.Serialize(),
                IdentityId = identityId,
            };

            await DisableRegistration(identityId);
            await ctx.PasswordAuthentications.AddAsync(auth);
            await ctx.SaveChangesAsync();

            return Ok(new AuthenticationResponse { IdentityId = identityId });
        }

        private async Task<StoredPassword> HashPassword(string password)
        {
            var target = hashingOptions.Value.Target;
            var salt = await hasher.GenerateSalt(target.SaltLength.Value);
            var pwHash = await hasher.Hash(password.ToBytes(), salt, target);

            return StoredPassword.From(pwHash, salt, target);
        }

        private async Task DisableRegistration(Guid identityId)
        {
            var registrations = await ctx.PasswordAuthentications.Where(x => x.IdentityId == identityId && x.DisabledAt == null).ToListAsync();
            foreach (var reg in registrations)
            {
                reg.DisabledAt = DateTime.UtcNow;
            }
        }
    }
}
