using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KEN.Startup))]
namespace KEN
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
