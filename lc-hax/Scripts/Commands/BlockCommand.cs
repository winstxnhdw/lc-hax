namespace Hax;

public class BlockCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /block <property>");
            return;
        }

        if (args[0] is "credits") {
            Settings.EnableBlockCredits = !Settings.EnableBlockCredits;

            Console.Print(
                $"{(Settings.EnableBlockCredits
                    ? "Blocking all incoming credits!"
                    : "No longer blocking credits!")}"
            );
        }

        else if (args[0] is "enemy") {
            Settings.EnableUntargetable = !Settings.EnableUntargetable;

            Console.Print(
                $"{(Settings.EnableUntargetable
                    ? "Enemies will no longer target you!"
                    : "Enemies can now target you!")}"
            );
        }

        else {
            Console.Print($"Invalid property!");
        }
    }
}
