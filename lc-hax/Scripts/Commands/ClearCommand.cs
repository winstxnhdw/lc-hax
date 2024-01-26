using Hax;

[Command("/clear")]
public class ClearCommand : ICommand {
    public void Execute(StringArray _) {
        Chat.Clear();
    }
}
