using System.Threading;
using System.Threading.Tasks;

class PrivilegedCommand(ICommand command) : ICommand {
    ICommand Command { get; } = command;

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { IsHost: true }) {
            Chat.Print("You must be the host to use this command!");
            return;
        }

        await this.Command.Execute(args, cancellationToken);
    }
}
