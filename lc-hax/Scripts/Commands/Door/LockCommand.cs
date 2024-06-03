[Command("lock")]
class LockCommand : ICommand, ISecureGate {
    public void Execute(StringArray _) {
        this.SetSecureDoorState(false);
        Chat.Print("All gates locked!");
    }
}
