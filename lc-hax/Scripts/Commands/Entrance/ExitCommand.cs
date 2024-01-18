[Command("/exit")]
public class ExitCommand : IEntrance, ICommand {
    public void Execute(StringArray _) {
        this.EntranceTeleport(true);
    }
}
