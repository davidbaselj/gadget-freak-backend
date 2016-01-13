using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(gadget_freak_backend.Startup))]
namespace gadget_freak_backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
