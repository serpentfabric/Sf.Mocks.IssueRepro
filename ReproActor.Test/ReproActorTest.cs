using Microsoft.ServiceFabric.Actors;
using ServiceFabric.Mocks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReminderExceptionRepro.Test
{
    public class ReproActorTest
    {
        [Fact]
        public async Task TestBehaviorDiffersFromActualRuntime()
        {
            // arrange
            var svc = MockActorServiceFactory.CreateActorServiceForActor<ReproActor>();
            var actor = svc.Activate(new ActorId(string.Empty));
            await actor.ArrangeAsync(default);

            // act
            await actor.ReceiveReminderAsync(nameof(ReproActor),
                Array.Empty<byte>(),
                TimeSpan.MaxValue,
                TimeSpan.MaxValue);

            // assert
            await actor.AssertAsync(default);
        }
    }
}
