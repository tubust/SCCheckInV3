using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SCCheckinV3.Startup))]
namespace SCCheckinV3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
