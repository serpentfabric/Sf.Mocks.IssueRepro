using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace ReminderExceptionRepro.Interfaces
{
    public interface IReproActor : IActor
    {
        Task RegisterNonRepeatingReminderAsync(CancellationToken cancellationToken);

        Task UnregisterNonRepeatingReminderAsync(CancellationToken cancellationToken);
    }
}
