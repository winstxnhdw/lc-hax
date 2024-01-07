using Hax;

[Command("/open")]
public class OpenCommand : ICommand {
    public void Execute(string[] args) {
        Helper.CloseShipDoor(false);
    }
}
