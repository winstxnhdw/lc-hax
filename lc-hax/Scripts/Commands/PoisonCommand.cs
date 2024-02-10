using GameNetcodeStuff;
using Hax;

[Command("poison")]
internal class PoisonCommand : ICommand {
    void PoisonPlayer(PlayerControllerB player, int damage, ulong delay, ulong duration) =>
        Helper.CreateComponent<TransientBehaviour>()
              .Init(_ => player.DamagePlayerRpc(damage), duration, delay);

    public void Execute(StringArray args) {
        if (args.Length < 4) {
            Chat.Print("Usage: poison <player> <damage> <delay> <duration>");
            Chat.Print("Usage: poison --all <damage> <delay> <duration>");
            return;
        }

        if (!int.TryParse(args[1], out int damage)) {
            Chat.Print("Invalid damage!");
            return;
        }

        if (!ulong.TryParse(args[2], out ulong delay)) {
            Chat.Print("Invalid delay!");
            return;
        }

        if (!ulong.TryParse(args[3], out ulong duration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        if (args[0] is "--all") {
            Helper.ActivePlayers.ForEach(player => this.PoisonPlayer(player, damage, delay, duration));
        }

        else if (Helper.GetActivePlayer(args[0]) is PlayerControllerB player) {
            this.PoisonPlayer(player, damage, delay, duration);
        }

        else {
            Chat.Print("Target player is not alive or found!");
        }
    }
}
