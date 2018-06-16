using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAAP.Context;
using OpenAAP.Errors;
using OpenAAP.Requests;

namespace OpenAAP.Controllers
{
    [Route("identity")]
    public class IdentityController : Controller
    {
        private readonly OpenAAPContext ctx;

        public IdentityController(
            OpenAAPContext context
        )
        {
            ctx = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> All()
        {
            return Ok(await ctx.Identities.ToListAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Identity))]
        [ProducesResponseType(404, Type = typeof(IdentityNotFound))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var identity = await ctx.Identities.FindAsync(id);

            if (identity == null)
            {
                return NotFound(new IdentityNotFound());
            }

            return Ok(identity);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Identity))]
        public async Task<IActionResult> Create([FromBody]CreateIdentityRequest create)
        {
            var identity = new Identity
            {
                Description = create.Description,
                Email = create.Email,
                UserName = create.UserName
            };

            ctx.Identities.Add(identity);

            await ctx.SaveChangesAsync();

            return Ok(identity);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(Identity))]
        [ProducesResponseType(404, Type = typeof(IdentityNotFound))]
        public async Task<IActionResult> Update(Guid id, [FromBody]UpdateIdentityRequest update)
        {
            var identity = await ctx.Identities.FindAsync(id);

            if (identity == null)
            {
                return NotFound(new IdentityNotFound());
            }

            identity.Description = update.Description;
            identity.Email = update.Email;
            identity.UserName = update.UserName;

            await ctx.SaveChangesAsync();

            return Ok(identity);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404, Type = typeof(IdentityNotFound))]
        public async Task<IActionResult> Delete(Guid id)
        {
            var identity = await ctx.Identities.FindAsync(id);

            if (identity == null)
            {
                return NotFound(new IdentityNotFound());
            }

            ctx.Identities.Remove(identity);

            await ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}
