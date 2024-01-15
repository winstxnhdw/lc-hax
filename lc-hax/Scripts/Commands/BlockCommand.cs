using System;
using Hax;

[Command("/block")]
public class BlockCommand : ICommand {
    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /block <property>");
            return;
        }

        if (args[0] is "credits") {
            Setting.EnableBlockCredits = !Setting.EnableBlockCredits;

            Chat.Print(
                $"{(Setting.EnableBlockCredits
                    ? "Blocking all incoming credits!"
                    : "No longer blocking credits!")}"
            );
        }

        else if (args[0] is "enemy") {
            Setting.EnableUntargetable = !Setting.EnableUntargetable;

            Chat.Print(
                $"{(Setting.EnableUntargetable
                    ? "Enemies will no longer target you!"
                    : "Enemies can now target you!")}"
            );
        }

        else if (args[0] is "radar") {
            Setting.EnableBlockRadar = !Setting.EnableBlockRadar;

            Chat.Print(
                $"{(Setting.EnableBlockRadar
                    ? "Blocking radar targets!"
                    : "No longer blocking radar targets!")}"
            );
        }

        else {
            Chat.Print($"Invalid property!");
        }
    }
}
