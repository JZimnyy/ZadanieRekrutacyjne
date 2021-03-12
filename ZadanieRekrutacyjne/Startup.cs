using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ZadanieRekrutacyjne.Startup))]
namespace ZadanieRekrutacyjne
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
