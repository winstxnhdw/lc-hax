[Command("/lock")]
public class LockCommand : ICommand, ISecureGate {
    public void Execute(StringArray _) {
        this.SetSecureDoorState(false);
    }
}
