using Microsoft.Owin;
using Owin;
using steamStats.Models;
using System.Web.Services.Description;

[assembly: OwinStartupAttribute(typeof(steamStats.Startup))]
namespace steamStats
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        
    }
}
