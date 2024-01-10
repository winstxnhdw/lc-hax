using Hax;

[Command("/close")]
public class CloseCommand : ICommand, IShipDoor {
    public void Execute(string[] args) {
        this.CloseShipDoor(true);
    }
}
