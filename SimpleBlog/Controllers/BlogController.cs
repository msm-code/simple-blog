using SimpleBlog.Models;
using SimpleBlog.Core;
using System.Web.Mvc;
using System.Web;

namespace SimpleBlog.Controllers {
    public class BlogController : Controller {
        private readonly IBlogRepository blogRepository;

        public BlogController(IBlogRepository blogRepository) {
            this.blogRepository = blogRepository;
        }

        public ViewResult Posts(int p = 1) {
            var viewModel = new ListViewModel(blogRepository, p);

            ViewBag.Title = "Latest Posts";
            return View("List", viewModel);
        }

        public ViewResult Category(string category, int p = 1) {
            var viewModel = ListViewModel.FromCategory(blogRepository, category, p);
            if (viewModel.Category == null) {
                throw new HttpException(404, "Category not found");
            }

            ViewBag.Title = string.Format(@"Latest posts on category '{0}'",
                viewModel.Category.Name);
            return View("List", viewModel);
        }

        public ViewResult Tag(string tag, int p = 1) {
            var viewModel = ListViewModel.FromTag(blogRepository, tag, p);
            if (viewModel.Tag == null) {
                throw new HttpException(404, "Category not found");
            }

            ViewBag.Title = string.Format(@"Latest posts tagged '{0}'",
                viewModel.Category.Name);
            return View("List", viewModel);
        }

        public ViewResult Search(string s, int p = 1) {
            ViewBag.Title = string.Format(@"Lists of posts found for search text ""{0}""", s);

            var viewModel = ListViewModel.FromSearch(blogRepository, s, p);
            return View("List", viewModel);
        }

        public ViewResult Post(int year, int month, string title) {
            var post = blogRepository.Post(year, month, title);

            if (post == null) {
                throw new HttpException(404, "Post not found");
            }

            if (post.Published == false && User.Identity.IsAuthenticated == false) {
                throw new HttpException(401, "The post is not published");
            }

            return View(post);
        }

        [ChildActionOnly]
        public PartialViewResult Sidebars() {
            var widgetViewModel = new WidgetViewModel(blogRepository);
            return PartialView("_Sidebars", widgetViewModel);
        }
    }
}