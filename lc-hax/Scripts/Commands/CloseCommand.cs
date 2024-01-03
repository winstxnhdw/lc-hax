namespace Hax;

[Command("/close")]
public class CloseCommand : ICommand {
    public void Execute(string[] args) {
        Helper.CloseShipDoor(true);
    }
}
