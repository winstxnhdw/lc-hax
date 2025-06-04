using System.Threading;
using System.Threading.Tasks;

class DebugCommand(ICommand command) : ICommand {
    ICommand Command { get; } = command;

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Chat.Print("This debug command is for testing purposes and is not meant for use!");
        await this.Command.Execute(args, cancellationToken);
    }
}
