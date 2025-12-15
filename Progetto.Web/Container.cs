using Progetto.Services;
using Progetto.Services.Shared;
using Microsoft.Extensions.DependencyInjection;
using Progetto.Web.SignalR;

namespace Progetto.Web
{
    public class Container
    {
        public static void RegisterTypes(IServiceCollection container)
        {
            // Registration of all the database services you have
            container.AddScoped<SharedService>();

            // Registration of Buoni Digitali service
            container.AddScoped<BuoniDigitaliService>();

            // Registration of SignalR events
            container.AddScoped<IPublishDomainEvents, SignalrPublishDomainEvents>();
        }
    }
}
