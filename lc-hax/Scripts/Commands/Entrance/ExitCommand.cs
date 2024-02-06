[Command("/exit")]
internal class ExitCommand : IEntrance, ICommand {
    public void Execute(StringArray _) => this.EntranceTeleport(true);
}
