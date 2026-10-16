using System.Threading;
using System.Threading.Tasks;

[Command("prefix")]
sealed class PrefixCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: prefix <prefix>");
            return Task.CompletedTask;
        }

        if (!char.TryParse(args[0], out char prefix)) {
            Chat.Print("The prefix must be a single character!");
            return Task.CompletedTask;
        }

        State.CommandPrefix = prefix;
        Chat.Print($"The command prefix has been set to '{prefix}'");
        return Task.CompletedTask;
    }
}
