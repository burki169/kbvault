using System.Reflection;
using Autofac;
using Autofac.Integration.SignalR;
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
            var config = new HubConfiguration();            
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            var container = builder.Build();
            config.Resolver = new AutofacDependencyResolver(container);            
            //app.UseAutofacMiddleware(container);
            app.MapSignalR();
        }
    }
}