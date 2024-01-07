using Hax;

public class Debug(ICommand command) : ICommand {
    ICommand Command { get; } = command;

    public void Execute(string[] args) {
        Chat.Print("This debug command is for testing purposes and is not meant for use!");
        this.Command.Execute(args);
    }
}
