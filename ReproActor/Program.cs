using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using ReminderExceptionRepro.Interfaces;

namespace ReminderExceptionRepro
{
    internal static class Program
    {
        private static async Task Main()
        {
            try
            {
                await ActorRuntime.RegisterActorAsync<ReproActor>(
                   (context, actorType) => new ActorService(context, actorType));
                var actorProxyFactory = new ActorProxyFactory();
                var actor = actorProxyFactory.CreateActorProxy<IReproActor>(
                    new Uri("fabric:/ReminderExceptionRepro/ReproActorService"), 
                    new ActorId("repro"));
                await actor.ArrangeAsync(default);
                await Task.Delay(TimeSpan.FromSeconds(1));
                await actor.AssertAsync(default);
                await Task.Delay(Timeout.InfiniteTimeSpan);
            }
            catch (Exception e)
            {
                // this exeption proves the difference in behavior
                throw;
            }
        }
    }
}
