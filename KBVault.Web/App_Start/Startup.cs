using System.Reflection;
using Autofac;
using Autofac.Integration.SignalR;
using KBVault.Dal;
using KBVault.Dal.Repository;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(KBVault.Web.Startup))]

namespace KBVault.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TagRepository>().As<ITagRepository>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<ArticleRepository>().As<IArticleRepository>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            var config = new HubConfiguration();
            builder.RegisterHubs(typeof(Startup).Assembly).PropertiesAutowired();
            var container = builder.Build();
            config.Resolver = new AutofacDependencyResolver(container);
            app.UseAutofacMiddleware(container);
            app.MapSignalR("/signalr", config);
        }
    }
}