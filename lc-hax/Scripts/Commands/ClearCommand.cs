[Command("clear")]
class ClearCommand : ICommand {
    public void Execute(StringArray _) => Chat.Clear();
}
