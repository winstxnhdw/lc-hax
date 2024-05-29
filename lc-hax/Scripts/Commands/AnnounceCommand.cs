[Command("announce")]
internal class AnnounceCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usage: /announce <message>");
            return;
        }

        var message = string.Join(" ", args[0..]);

        Chat.Announce(string.Join(' ', message), true);
    }
}