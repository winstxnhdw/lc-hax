[Command("/close")]
public class CloseCommand : ICommand, IShipDoor {
    public void Execute(StringArray _) {
        this.SetShipDoorState(true);
    }
}
