using System;
using Hax;

[Command("/open")]
public class OpenCommand : ICommand, IShipDoor {
    public void Execute(ReadOnlySpan<string> _) {
        this.SetShipDoorState(false);
    }
}
