using Owin;

namespace com.web.server.weconnect.weconnectwebrole
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}