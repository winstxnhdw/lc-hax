
namespace Hax;

public class ShovelCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Helper.PrintSystem("Usage: /shovel <force=1>");
            return;
        }

        if (!int.TryParse(args[0], out int shovelHitForce)) {
            Helper.PrintSystem("Invalid valuer!");
            return;
        }

        Settings.ShovelHitForce = shovelHitForce;
        Helper.PrintSystem($"Shovel hit force is now set to {shovelHitForce}!");
    }
}
