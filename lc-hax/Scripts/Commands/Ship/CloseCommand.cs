[Command("close")]
class CloseCommand : ICommand, IShipDoor {
    public void Execute(StringArray _) => this.SetShipDoorState(true);
}
