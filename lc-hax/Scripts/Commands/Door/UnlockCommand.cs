using System.Threading;
using System.Threading.Tasks;

[Command("unlock")]
sealed class UnlockCommand : ICommand, ISecureDoor {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        this.SetSecureDoorState(true);
        Helper.FindObjects<DoorLock>()
              .ForEach(door => door.UnlockDoorSyncWithServer());

        Chat.Print("All doors unlocked!");
    }
}
