internal class DebugCommand(ICommand command) : ICommand
{
    private ICommand Command { get; } = command;

    public void Execute(StringArray args)
    {
        Command.Execute(args);
    }
}