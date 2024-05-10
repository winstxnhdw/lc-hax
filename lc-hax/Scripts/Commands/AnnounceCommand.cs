using Hax;

[Command("announce")]
class AnnounceCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usage: /announce <message>");
            return;
        }

        Chat.Announce(string.Join(' ', args.Span.ToArray(), true));
    }
}