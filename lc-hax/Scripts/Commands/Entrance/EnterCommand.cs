[Command("/enter")]
public class EnterCommand : IEntrance, ICommand {
    public void Execute(StringArray _) {
        this.EntranceTeleport(false);
    }
}
