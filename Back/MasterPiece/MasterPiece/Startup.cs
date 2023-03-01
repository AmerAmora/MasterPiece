using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MasterPiece.Startup))]
namespace MasterPiece
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
