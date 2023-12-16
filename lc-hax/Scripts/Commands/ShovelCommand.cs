
namespace Hax;

public class ShovelCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /shovel <force=1>");
            return;
        }

        if (!int.TryParse(args[0], out int shovelHitForce)) {
            Console.Print("Invalid value!");
            return;
        }

        Settings.ShovelHitForce = shovelHitForce;
        Console.Print($"Shovel hit force is now set to {shovelHitForce}!");
    }
}
