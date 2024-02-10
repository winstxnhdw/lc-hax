using Hax;

[Command("shovel")]
internal class ShovelCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: shovel <force=1>");
            return;
        }

        if (!ushort.TryParse(args[0], out ushort shovelHitForce)) {
            Chat.Print("Invalid value!");
            return;
        }

        State.ShovelHitForce = shovelHitForce;
        Chat.Print($"Shovel hit force is now set to {shovelHitForce}!");
    }
}
