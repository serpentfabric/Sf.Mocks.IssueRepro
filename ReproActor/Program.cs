using System;
using System.Diagnostics.Contracts;
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
        private static ActorProxyFactory _actorProxyFactory = new ActorProxyFactory();
        private static Uri _actorServiceUri = new Uri("fabric:/ReminderExceptionRepro/ReproActorService");

        private static async Task Main()
        {
            //arrange
            await ActorRuntime.RegisterActorAsync<ReproActor>(
               (context, actorType) => new ActorService(context, actorType));
            var actor = _actorProxyFactory.CreateActorProxy<IReproActor>(
                _actorServiceUri, new ActorId("repro"));

            //act
            await actor.RegisterNonRepeatingReminderAsync(default);
            //reminder expires some nonzero in future but before this delay is over
            await Task.Delay(Constants.DelayedNeededToInduceError);

            //assert (throws exception)
            try { await actor.UnregisterNonRepeatingReminderAsync(default); }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    if (e is ReminderNotFoundException)
                    {
                        return false;
                    }
                    return true;
                });
            }
        }
    }
}
