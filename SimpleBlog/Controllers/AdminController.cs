using Newtonsoft.Json;
using SimpleBlog.Core;
using SimpleBlog.Core.Objects;
using SimpleBlog.Models;
using SimpleBlog.Providers;
using System;
using System.Text;
using System.Linq;
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

        [HttpPost, ValidateInput(false)]
        public ContentResult AddPost(Post post) {
            string json;

            ModelState.Clear();
            if (TryValidateModel(post)) {
                var id = blogRepository.AddPost(post);

                json = JsonConvert.SerializeObject(new {
                    id = id,
                    success = true,
                    message = "Post added successfully"
                });
            } else {
                json = JsonConvert.SerializeObject(new {
                    id = 0,
                    success = false,
                    message = "Failed to add the post"
                });
            }

            return Content(json, "application/json");
        }

        [HttpPost, ValidateInput(false)]
        public ContentResult EditPost(Post post) {
            string json;

            ModelState.Clear();
            if (TryValidateModel(post)) {
                blogRepository.EditPost(post);
                json = JsonConvert.SerializeObject(new {
                    id = post.Id,
                    success = true,
                    message = "Changes saved successfully."
                });
            } else {
                json = JsonConvert.SerializeObject(new {
                    id = 0,
                    success = false,
                    message = "Failed to save the changes."
                });
            }

            return Content(json, "application/json");
        }

        [HttpPost]
        public ContentResult DeletePost(int id) {
            blogRepository.DeletePost(id);

            var json = JsonConvert.SerializeObject(new {
                id = 0,
                success = true,
                message = "Post deleted successfully."
            });

            return Content(json, "application/json");
        }

        public ContentResult GetCategoriesHtml() {
            var categories = blogRepository.Categories().OrderBy(s => s.Name);

            var sb = new StringBuilder();
            sb.AppendLine(@"<select>");

            foreach (var category in categories) {
                sb.AppendLine(string.Format(@"<option value=""{0}"">{1}</option>",
                    category.Id, category.Name));
            }

            sb.AppendLine("<select>");
            return Content(sb.ToString(), "text/html");
        }

        public ContentResult GetTagsHtml() {
            var tags = blogRepository.Tags().OrderBy(s => s.Name);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<select multiple=""multiple"">");

            foreach (var tag in tags) {
                sb.AppendLine(string.Format(@"<option value=""{0}"">{1}</option>",
                    tag.Id, tag.Name));
            }

            sb.AppendLine("<select>");
            return Content(sb.ToString(), "text/html");
        }
    }
}