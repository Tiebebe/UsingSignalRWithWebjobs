using Microsoft.Owin;
using Owin;
using Website;

[assembly: OwinStartup(typeof(Startup))]

namespace Website
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
