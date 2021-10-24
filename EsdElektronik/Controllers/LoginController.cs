using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EsdElektronik.Models.Entity;
namespace EsdElektronik.Controllers
{    
    public class LoginController : Controller
    {
        esdelektronikEntities esd = new esdelektronikEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(tbl_users u1)
        {
            var control = esd.tbl_users.Where(x => (x.username == u1.username) & (x.password == u1.password)).FirstOrDefault();
            if (control != null)
            {
                FormsAuthentication.SetAuthCookie(control.username, true);
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
    }
}