using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace PharmaCO.Models
{

    namespace PharmaCO.Models
    {
        public class ApplicationUser : IdentityUser
        {
            public string FullName { get; set; }
            public string Address { get; set; }
        }
    }

}