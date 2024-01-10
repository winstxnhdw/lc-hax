using Hax;

[Command("/open")]
public class OpenCommand : ICommand, IShipDoor {
    public void Execute(string[] args) {
        this.SetShipDoorState(false);
    }
}
