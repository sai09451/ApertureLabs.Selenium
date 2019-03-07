using ApertureLabs.Selenium.PageObjects;
using Autofac;
using MockServer.PageObjects.Home;
using MockServer.PageObjects.Widget;

namespace MockServer.PageObjects
{
    public class DependencyRegistrar : Module, IOrderedModule
    {
        public int Order => 0;

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<BasePage>()
                .As<IBasePage>();
            builder
                .RegisterType<HomePage>()
                .AsSelf();
            builder
                .RegisterType<WidgetPage>()
                .AsSelf();

            base.Load(builder);
        }
    }
}
