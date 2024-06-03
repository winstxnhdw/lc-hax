#region

using Hax;

#endregion

class PrivilegedCommand(ICommand command) : ICommand {
    ICommand Command { get; } = command;

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not { IsHost: true }) {
            Chat.Print("You must be the host to use this command!");
            return;
        }

        this.Command.Execute(args);
    }
}
