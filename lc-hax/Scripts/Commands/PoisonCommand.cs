using GameNetcodeStuff;
using Hax;

[Command("/poison")]
public class PoisonCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 4) {
            Chat.Print("Usage: /poison <player> <damage> <delay> <duration>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Player not found!");
            return;
        }

        if (!int.TryParse(args[1], out int damage)) {
            Chat.Print("Invalid damage!");
            return;
        }

        if (!int.TryParse(args[2], out int delay)) {
            Chat.Print("Invalid delay!");
            return;
        }

        if (!int.TryParse(args[3], out int duration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        _ = Helper.CreateComponent<TransientBehaviour>()
                  .Init(_ => player.DamagePlayerRpc(damage), duration, delay);
    }
}
