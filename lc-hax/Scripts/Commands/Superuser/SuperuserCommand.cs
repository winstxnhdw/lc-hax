using Hax;

public class SuperuserCommand(ICommand command) : ICommand {
    ICommand Command { get; } = command;

    public void Execute(StringArray args) {
        if (!Setting.IsSuperuser) {
            Chat.Print("You must be a superuser to use this command!");
            return;
        }

        this.Command.Execute(args);
    }
}
