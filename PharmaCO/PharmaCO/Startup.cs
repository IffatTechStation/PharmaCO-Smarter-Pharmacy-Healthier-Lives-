using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PharmaCO.Startup))]
namespace PharmaCO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
