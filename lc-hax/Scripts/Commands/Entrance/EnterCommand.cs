using System;
using Hax;

[Command("/enter")]
public class EnterCommand : IEntrance, ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        this.EntranceTeleport(false);
    }
}
