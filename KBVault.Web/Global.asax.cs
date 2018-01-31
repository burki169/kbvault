using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using KBVault.Dal;
using KBVault.Dal.Repository;
using KBVault.Web.Business.ApplicationSettings;
using KBVault.Web.Business.Articles;
using KBVault.Web.Business.Categories;
using KBVault.Web.ViewEngines;
using Microsoft.AspNet.SignalR;
using AutofacDependencyResolver = Autofac.Integration.Mvc.AutofacDependencyResolver;

namespace KBVault.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CategoryFactory>().As<ICategoryFactory>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<TagRepository>().As<ITagRepository>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<ArticleRepository>().As<IArticleRepository>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<ArticleFactory>().As<IArticleFactory>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<SettingsFactory>().As<ISettingsFactory>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterType<SettingsService>().As<ISettingsService>().AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            System.Web.Mvc.ViewEngines.Engines.Clear();
            System.Web.Mvc.ViewEngines.Engines.Add(new KbVaultViewEngine());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}