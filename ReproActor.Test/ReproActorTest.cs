using Microsoft.ServiceFabric.Actors;
using ServiceFabric.Mocks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReminderExceptionRepro.Test
{
    public class ReproActorTest
    {
        private MockActorService<ReproActor> _actorService;
        private ReproActor _actor;
        private TaskCompletionSource<bool> _reminderRan = new TaskCompletionSource<bool>();

        public ReproActorTest()
        {
            _actorService = MockActorServiceFactory.CreateActorServiceForActor<ReproActor>();
            _actor = _actorService.Activate(new ActorId(string.Empty));
            _actor.InvokeOnActivateAsync().Wait();
        }

        [Fact]
        public async Task TestBehaviorDiffersFromActualRuntime()
        {
            // arrange
            await _actor.RegisterNonRepeatingReminderAsync(default);

            // act
            await _actor.ReceiveReminderAsync(nameof(ReproActor),
                Array.Empty<byte>(),
                TimeSpan.MaxValue,
                TimeSpan.MaxValue);
            await Task.Delay(Constants.DelayedNeededToInduceError);

            // assert
            await Assert.ThrowsAsync<ReminderNotFoundException>(async () =>
                await _actor.UnregisterNonRepeatingReminderAsync(default));
        }
    }
}
