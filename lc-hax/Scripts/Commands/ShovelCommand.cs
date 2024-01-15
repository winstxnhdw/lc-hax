using System;
using Hax;

[Command("/shovel")]
public class ShovelCommand : ICommand {
    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /shovel <force=1>");
            return;
        }

        if (!ushort.TryParse(args[0], out ushort shovelHitForce)) {
            Chat.Print("Invalid value!");
            return;
        }

        Setting.ShovelHitForce = shovelHitForce;
        Chat.Print($"Shovel hit force is now set to {shovelHitForce}!");
    }
}
