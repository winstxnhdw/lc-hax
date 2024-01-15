using System;
using Hax;

[Command("/hitclick")]
public class HitOnClickCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        Setting.EnableHitOnLeftClick = !Setting.EnableHitOnLeftClick;
        Chat.Print($"Hitclick: {(Setting.EnableHitOnLeftClick ? "Enabled" : "Disabled")}");
    }
}
