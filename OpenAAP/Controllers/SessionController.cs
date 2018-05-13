 using Microsoft.AspNetCore.Mvc;
using OpenAAP.Context;
using OpenAAP.Errors;
using OpenAAP.Services.SessionStorage;
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
        private readonly SessionService sessionSvc;

        public SessionController(
             OpenAAPContext context,
             SessionService session
        )
        {
            ctx = context;
            this.sessionSvc = session;
        }

        [HttpGet("{sessionId}")]
        [ProducesResponseType(200, Type = typeof(ISession))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetSession(Guid sessionId)
        {
            var session = await sessionSvc.LookupBySessionId(sessionId);
            
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
            var session = await sessionSvc.LookupBySessionId(sessionId);

            if (session != null)
            {
                await sessionSvc.DeleteBySessionId(sessionId);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
