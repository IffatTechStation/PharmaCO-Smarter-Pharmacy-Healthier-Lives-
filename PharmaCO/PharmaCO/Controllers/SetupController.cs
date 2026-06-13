using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PharmaCO.Models;
using System.Web.Mvc;

namespace PharmaCO.Controllers
{
    public class SetupController : Controller
    {
        public ActionResult CreateRoles()
        {
            using (var context = new PharmaCODb())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                if (!roleManager.RoleExists("Owner"))
                {
                    roleManager.Create(new IdentityRole("Owner"));
                }

                if (!roleManager.RoleExists("SalesMan"))
                {
                    roleManager.Create(new IdentityRole("SalesMan"));
                }
            }

            return Content("Owner and SalesMan role created!!!");
        }
    }
}
