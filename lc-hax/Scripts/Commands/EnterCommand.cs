using Hax;

[Command("/enter")]
public class EnterCommand : IEntrance, ICommand {
    public void Execute(string[] args) {
        this.EntranceTeleport(false);
    }
}
