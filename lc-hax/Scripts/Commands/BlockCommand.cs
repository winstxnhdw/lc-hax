namespace Hax;

[Command("/block")]
public class BlockCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /block <property>");
            return;
        }

        if (args[0] is "credits") {
            Setting.EnableBlockCredits = !Setting.EnableBlockCredits;

            Console.Print(
                $"{(Setting.EnableBlockCredits
                    ? "Blocking all incoming credits!"
                    : "No longer blocking credits!")}"
            );
        }

        else if (args[0] is "enemy") {
            Setting.EnableUntargetable = !Setting.EnableUntargetable;

            Console.Print(
                $"{(Setting.EnableUntargetable
                    ? "Enemies will no longer target you!"
                    : "Enemies can now target you!")}"
            );
        }

        else {
            Console.Print($"Invalid property!");
        }
    }
}
