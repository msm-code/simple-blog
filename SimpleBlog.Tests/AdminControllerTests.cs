using Moq;
using SimpleBlog.Controllers;
using SimpleBlog.Models;
using SimpleBlog.Providers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace SimpleBlog.Tests {
    public class AdminControllerTests {
        AdminController adminController;
        Mock<IAuthProvider> authProvider;

        public AdminControllerTests() {
            authProvider = new Mock<IAuthProvider>();
            adminController = new AdminController(authProvider.Object);

            var httpContextMock = new Mock<HttpContextBase>();
            adminController.Url = new UrlHelper(new RequestContext(httpContextMock.Object, new RouteData()));
        }

        [Fact]
        public void IsLoggedIn_True_Test() {
            authProvider.Setup(s => s.IsLoggedIn).Returns(true);
            var actual = adminController.Login("/admin/manage");

            Assert.IsType<RedirectResult>(actual);
            Assert.Equal("/admin/manage", (actual as RedirectResult).Url);
        }

        [Fact]
        public void IsLoggedIn_False_Test() {
            authProvider.Setup(s => s.IsLoggedIn).Returns(false);
            var actual = adminController.Login("/");

            Assert.IsType<ViewResult>(actual);
            Assert.Equal("/", (actual as ViewResult).ViewBag.ReturnUrl);
        }

        [Fact]
        public void Post_Model_Invalid_Test() {
            var model = new LoginModel();
            adminController.ModelState.AddModelError("UserName", "Username is required");

            var actual = adminController.Login(model, "/");

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void Login_Post_User_Invalid_Test() {
            var model = new LoginModel {
                UserName = "invaliduser",
                Password = "password"
            };

            authProvider.Setup(s => s.Login(model.UserName, model.Password))
                        .Returns(false);

            var actual = adminController.Login(model, "/");

            Assert.IsType<ViewResult>(actual);
            var modelStateErrors = adminController.ModelState[""].Errors;
            Assert.True(modelStateErrors.Count > 0);
            Assert.Equal("Username or password is invalid", modelStateErrors[0].ErrorMessage);
        }

        [Fact]
        public void Post_User_Valid_Test() {
            var model = new LoginModel {
                UserName = "validuser",
                Password = "password"
            };
            authProvider.Setup(s => s.Login(model.UserName, model.Password))
                        .Returns(true);

            var actual = adminController.Login(model, "/");

            Assert.IsType<RedirectResult>(actual);
            Assert.Equal("/", (actual as RedirectResult).Url);
        }
    }
}
