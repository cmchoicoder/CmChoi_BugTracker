using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CmChoi_BugTracker.Startup))]
namespace CmChoi_BugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
