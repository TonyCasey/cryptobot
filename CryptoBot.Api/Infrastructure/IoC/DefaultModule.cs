using Api.CryptoBot.Controllers;
using Autofac;

namespace Api.CryptoBot.Infrastructure.IoC
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<BasicDependency>().As<IBasicDependency>();
        }
    }
}