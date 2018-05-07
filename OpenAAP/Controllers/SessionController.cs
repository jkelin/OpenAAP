using Microsoft.AspNetCore.Mvc;
using OpenAAP.Context;
using OpenAAP.Errors;
using OpenAAP.Services.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Controllers
{
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly OpenAAPContext ctx;
        private readonly ISessionStorageService sessionStorage;

        public SessionController(
             OpenAAPContext context,
             ISessionStorageService sessionStorage
        )
        {
            ctx = context;
            this.sessionStorage = sessionStorage;
        }

        [HttpGet("{sessionId}")]
        [ProducesResponseType(200, Type = typeof(SessionModel))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetSession(Guid sessionId)
        {
            var session = await sessionStorage.LookupSessionBySessionId(sessionId);
            
            if (session != null)
            {
                return Ok(session);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{sessionId}")]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteSession(Guid sessionId)
        {
            var session = await sessionStorage.LookupSessionBySessionId(sessionId);

            if (session != null)
            {
                await sessionStorage.DeleteSessionBySessionId(sessionId);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
