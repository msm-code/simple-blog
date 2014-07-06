using Ninject;
using Ninject.Web.Common;
using SimpleBlog.Core;
using SimpleBlog.Core.Objects;
using SimpleBlog.Providers;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleBlog {
    public class MvcApplication : NinjectHttpApplication {
        protected override IKernel CreateKernel() {
            var kernel = new StandardKernel();

            kernel.Load(new RepositoryModule());
            kernel.Bind<IBlogRepository>().To<BlogRepository>();
            kernel.Bind<IAuthProvider>().To<AuthProvider>();

            return kernel;
        }

        protected override void OnApplicationStarted() {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(Post), new PostModelBinder(Kernel));
            base.OnApplicationStarted();
        }
    }
}
