using System;
using Hax;

[Command("/block")]
public class BlockCommand : ICommand {
    string BlockCredit() {
        Setting.EnableBlockCredits = !Setting.EnableBlockCredits;

        return $"{(Setting.EnableBlockCredits
            ? "Blocking all incoming credits!"
            : "No longer blocking credits!")}";
    }

    string BlockEnemy() {
        Setting.EnableUntargetable = !Setting.EnableUntargetable;

        return $"{(Setting.EnableUntargetable
            ? "Enemies will no longer target you!"
            : "Enemies can now target you!")}";
    }

    string BlockRadar() {
        Setting.EnableBlockRadar = !Setting.EnableBlockRadar;

        return $"{(Setting.EnableBlockRadar
            ? "Blocking radar targets!"
            : "No longer blocking radar targets!")}";
    }

    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /block <property>");
            return;
        }

        string result = args[0] switch {
            "credit" => this.BlockCredit(),
            "enemy" => this.BlockEnemy(),
            "radar" => this.BlockRadar(),
            _ => "Invalid property!"
        };

        Chat.Print(result);
    }
}
