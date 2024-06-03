class DebugCommand(ICommand command) : ICommand {
    ICommand Command { get; } = command;

    public void Execute(StringArray args) => this.Command.Execute(args);
}
