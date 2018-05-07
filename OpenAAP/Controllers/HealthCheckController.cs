using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAAP.Context;
using OpenAAP.Requests;

namespace OpenAAP.Controllers
{
    [Route("health")]
    public class HealthCheckController : Controller
    {
        private readonly OpenAAPContext ctx;

        public HealthCheckController(OpenAAPContext context)
        {
            ctx = context;
        }

        [HttpGet("ping")]
        [ProducesResponseType(200, Type = typeof(string))]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpPost("body-validator")]
        [ProducesResponseType(200, Type = typeof(ValidatorRequest))]
        public IActionResult Validator([FromBody] ValidatorRequest req)
        {
            return Ok(req);
        }
    }
}
