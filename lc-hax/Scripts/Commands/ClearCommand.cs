using Hax;

[Command("/clear")]
internal class ClearCommand : ICommand {
    public void Execute(StringArray _) => Chat.Clear();
}
