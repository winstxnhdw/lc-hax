using Hax;

internal class PrivilegedCommand(ICommand command) : ICommand
{
    private ICommand Command { get; } = command;

    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not { IsHost: true })
        {
            Chat.Print("You must be the host to use this command!");
            return;
        }

        Command.Execute(args);
    }
}