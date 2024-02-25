using Hax;

[Command("prefix")]
class PrefixCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: prefix <prefix>");
            return;
        }

        if (!char.TryParse(args[0], out char prefix)) {
            Chat.Print("The prefix must be a single character!");
            return;
        }

        State.CommandPrefix = prefix;
        Chat.Print($"The command prefix has been set to '{prefix}'");
    }
}
