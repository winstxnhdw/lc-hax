using System;
using Hax;

[Command("/close")]
public class CloseCommand : ICommand, IShipDoor {
    public void Execute(ReadOnlySpan<string> _) {
        this.SetShipDoorState(true);
    }
}
