using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using seguridad.Attributes;
using seguridad.Models;

namespace seguridad.Controllers
{
    public class HomeController : Controller
    {

        [AllowAnonymous]
        public IActionResult Index([FromServices]IDistributedCache cache)
        {

            var user = this.GetUserInfo();


            HttpContext.Session.SetString("value","prueba");
          

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }


        ///[CacheMethod()]
        ApplicationUser GetUserInfo()
        {
            return new ApplicationUser();
        }



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
