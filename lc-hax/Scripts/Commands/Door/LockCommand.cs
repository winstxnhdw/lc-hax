using System;
using Hax;

[Command("/lock")]
public class LockCommand : ICommand, ISecureGate {
    public void Execute(ReadOnlySpan<string> _) {
        this.SetSecureDoorState(false);
    }
}
