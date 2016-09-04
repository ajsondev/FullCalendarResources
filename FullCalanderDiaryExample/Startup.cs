using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FullCal.Startup))]
namespace FullCal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
