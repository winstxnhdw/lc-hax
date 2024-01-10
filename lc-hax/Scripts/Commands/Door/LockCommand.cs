using Hax;

[Command("/lock")]
public class LockCommand : ICommand, ISecureGate {
    public void Execute(string[] args) {
        this.SetSecureDoorState(false);
    }
}
