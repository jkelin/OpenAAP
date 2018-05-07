using Microsoft.AspNetCore.Mvc;
using OpenAAP.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Controllers
{
    public class GlobalController : Controller
    {
        [Route("{*url}", Order = 999)]
        public IActionResult CatchAll()
        {
            return NotFound(new RouteNotFound());
        }
    }
}
