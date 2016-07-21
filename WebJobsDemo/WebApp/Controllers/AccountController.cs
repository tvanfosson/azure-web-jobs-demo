using System.Web.Mvc;
using System.Web.Security;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string returnUrl)
        {
            // TODO: yeah, don't do this - strictly for demo purposes
            FormsAuthentication.SetAuthCookie(username, false);

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(returnUrl);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
    }
}