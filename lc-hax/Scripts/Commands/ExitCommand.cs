namespace Hax;

public class ExitCommand : IEntrance, ICommand {
    public void Execute(string[] args) {
        this.EntranceTeleport(true);
    }
}
