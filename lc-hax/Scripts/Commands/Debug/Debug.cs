namespace Hax;

public class Debug(IDebugCommand debugCommand) : ICommand {
    IDebugCommand DebugCommand { get; } = debugCommand;

    public void Execute(string[] args) {
        Console.Print("This debug command is for testing purposes and is not meant for use!");
        this.DebugCommand.DebugExecute(args);
    }
}
