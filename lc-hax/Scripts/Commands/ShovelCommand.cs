
namespace Hax;

public class ShovelCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /shovel <force=1>");
            return;
        }

        Settings.ShovelHitForce = int.TryParse(args[1], out int shovelHitForce) ? shovelHitForce : Settings.ShovelHitForce;
    }
}
