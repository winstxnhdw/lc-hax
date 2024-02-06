[Command("/enter")]
internal class EnterCommand : IEntrance, ICommand {
    public void Execute(StringArray _) => this.EntranceTeleport(false);
}
