using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FoodPicker.Startup))]
namespace FoodPicker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
