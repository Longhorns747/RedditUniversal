using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UniversalRedditLogin.Startup))]
namespace UniversalRedditLogin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
