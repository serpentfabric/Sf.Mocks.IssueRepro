using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using ReminderExceptionRepro.Interfaces;

namespace ReminderExceptionRepro
{
    [StatePersistence(StatePersistence.Persisted)]
    public class ReproActor : Actor, IReproActor, IRemindable
    {
        private IActorReminder _reminder;

        private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        public TimeSpan NeverRepeatReminder => TimeSpan.FromMilliseconds(-1);

        public ReproActor(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        public async Task ArrangeAsync(CancellationToken cancellationToken)
        {
            _reminder = await RegisterReminderAsync(nameof(ReproActor),
                Array.Empty<byte>(),
                TimeSpan.Zero,
                NeverRepeatReminder);
        }

        public async Task AssertAsync(CancellationToken cancellationToken)
        {
            await _tcs.Task;
            await UnregisterReminderAsync(_reminder);
        }

        protected override Task OnActivateAsync()
        {
            base.OnActivateAsync();
            _reminder = GetReminder(nameof(ReproActor));
            return Task.CompletedTask;
        }

        public Task ReceiveReminderAsync(string reminderName,
            byte[] state,
            TimeSpan dueTime,
            TimeSpan period)
        {
            _tcs.TrySetResult(true);
            return Task.CompletedTask;
        }
    }
}
