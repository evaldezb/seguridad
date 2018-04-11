using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using seguridad.Models;

namespace seguridad.Controllers
{
    [Produces("application/json")]
    [Route("api/Default")]
    [AllowAnonymous]
    public class DefaultController : Controller
    {

        public IActionResult Get()
        {
            return Ok(new List<ApplicationUser>()
            {
                new ApplicationUser(){Email="email@email.com"},
                new ApplicationUser(){Email="email@email.com"},
                new ApplicationUser(){Email="email@email.com"},
                new ApplicationUser(){Email="email@email.com"},
                new ApplicationUser(){Email="email@email.com"},
                new ApplicationUser(){Email="email@email.com"},
                new ApplicationUser(){Email="email@email.com"},
                new ApplicationUser(){},
                new ApplicationUser(){},
                new ApplicationUser(){},
                new ApplicationUser(){Email="e"}
            });
        }

    }
}