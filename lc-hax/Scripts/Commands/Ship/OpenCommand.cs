[Command("/open")]
internal class OpenCommand : ICommand, IShipDoor {
    public void Execute(StringArray _) => this.SetShipDoorState(false);
}
