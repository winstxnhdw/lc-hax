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
            return;
        }
    }
}
