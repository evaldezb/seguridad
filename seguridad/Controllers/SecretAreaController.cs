using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace seguridad.Controllers
{
    public class CredicardModel
    {
        public string Tipo { get; set; }
        public string Numero { get; set; }
    }



    [Authorize(policy: "AdminPolicy")]
    public class SecretAreaController : Controller
    {

        IDataProtectionProvider protectionProvider;

        public SecretAreaController(IDataProtectionProvider protector)
        {
            protectionProvider = protector;
        }


        public IActionResult Index()
        {

            return View(new CredicardModel());
        }

        [HttpPost]
        public IActionResult Index(CredicardModel model)
        {

            var protector = protectionProvider.CreateProtector("UserKeysForCredicCard", User.Identity.Name);


            var xx = protector.Protect(model.Numero);

            protector.Unprotect(xx);

            return View(model);


        }
    }
}