[Command("/lock")]
internal class LockCommand : ICommand, ISecureGate {
    public void Execute(StringArray _) => this.SetSecureDoorState(false);
}
