using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SignalRTest.Startup))]
namespace SignalRTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
