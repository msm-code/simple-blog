using Newtonsoft.Json;
using SimpleBlog.Core;
using SimpleBlog.Models;
using SimpleBlog.Providers;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace SimpleBlog.Controllers {
    [Authorize]
    public class AdminController : Controller {
        readonly IAuthProvider authProvider;
        readonly IBlogRepository blogRepository;

        public AdminController(IAuthProvider authProvider, IBlogRepository blogRepository = null) {
            this.authProvider = authProvider;
            this.blogRepository = blogRepository;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            if (authProvider.IsLoggedIn) {
                return RedirectToLocalUrl(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl) {
            if (ModelState.IsValid && authProvider.Login(model.UserName, model.Password)) {
                return RedirectToLocalUrl(returnUrl);
            }

            ModelState.AddModelError("", "Username or password is invalid");
            return View();
        }

        public ActionResult Manage() {
            return View();
        }

        public ActionResult Logout() {
            authProvider.Logout();

            return RedirectToAction("Login", "Admin");
        }

        public ActionResult Posts(JqInViewModel jqParams) {
            var posts = blogRepository.Posts(jqParams.page - 1,
                jqParams.rows, jqParams.sidx, jqParams.sord == "asc");

            var totalPosts = blogRepository.TotalPosts(false);

            return Content(JsonConvert.SerializeObject(new {
                page = jqParams.page,
                records = totalPosts,
                rows = posts,
                total = Math.Ceiling((double)totalPosts / jqParams.rows)
            }), "application/json");
        }

        ActionResult RedirectToLocalUrl(string url) {
            if (Url.IsLocalUrl(url)) {
                return Redirect(url);
            } else {
                return RedirectToAction("Manage");
            }
        }
    }
}