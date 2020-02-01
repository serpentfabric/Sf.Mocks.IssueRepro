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

        public TimeSpan NeverRepeatReminder => TimeSpan.FromMilliseconds(-1);

        public ReproActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task RegisterNonRepeatingReminderAsync(CancellationToken cancellationToken)
        {
            _reminder = await RegisterReminderAsync(nameof(ReproActor),
                Array.Empty<byte>(),
                TimeSpan.FromSeconds(1),
                NeverRepeatReminder);
        }

        public async Task UnregisterNonRepeatingReminderAsync(CancellationToken cancellationToken)
        {
            await UnregisterReminderAsync(_reminder);
        }

        protected override Task OnActivateAsync()
        {
            base.OnActivateAsync();
            try
            {
                _reminder = GetReminder(nameof(ReproActor));
            }
            catch (ReminderNotFoundException rmne)
            {
                //squelch purposely here
                //if this reminder was not set before the actor was deactivated, then this exception is thrown
                //if this reminder had been set before the actor was deactivated, then no exception need be caught
            }
            return Task.CompletedTask;
        }

        public Task ReceiveReminderAsync(string reminderName,
            byte[] state,
            TimeSpan dueTime,
            TimeSpan period)
        {
            return Task.CompletedTask;
        }
    }
}
