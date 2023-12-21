namespace Hax;

public class DebugCommand : ICommand {
    public void Execute(string[] args) {
        Console.Print("This debug command is for testing purposes and is not meant for use!");
    }
}
