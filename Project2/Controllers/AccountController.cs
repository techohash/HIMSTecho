using Project2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project2.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(TBL_USER_D user)
        {
            PathologyEntities db = new PathologyEntities();
            var usr = db.TBL_USER_D.SingleOrDefault(x => x.USER_NAME == user.USER_NAME && x.USER_PASSWORD == user.USER_PASSWORD);
            if (usr != null)
            {

                if (usr.org == "admin")
                {
                    FormsAuthentication.SetAuthCookie(usr.USER_NAME, false);
                    return RedirectToAction("Index", "Home", new { user = user.USER_NAME });
                }
                else if (usr.USER_NAME == "st")
                {
                    FormsAuthentication.SetAuthCookie(usr.USER_NAME.ToString(), false);
                    return RedirectToAction("StudentDashBaord", "Home");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(usr.USER_NAME, false);
                    return RedirectToAction("staffDashboard", "Home", new { id = usr.USER_NAME });
                }

            }
            else
            {
                ViewBag.triedOnce = "yes";
                return View();
            }
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

    }
}