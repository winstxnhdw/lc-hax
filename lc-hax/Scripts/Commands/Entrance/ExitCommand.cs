using System;
using Hax;

[Command("/exit")]
public class ExitCommand : IEntrance, ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        this.EntranceTeleport(true);
    }
}
