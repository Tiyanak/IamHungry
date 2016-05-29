using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IamHungry.Startup))]
namespace IamHungry
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
