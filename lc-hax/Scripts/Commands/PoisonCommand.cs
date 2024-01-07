using GameNetcodeStuff;

namespace Hax;

[Command("/poison")]
public class PoisonCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 4) {
            Console.Print("Usage: /poison <player> <damage> <delay> <duration>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
            Console.Print("Player not found!");
            return;
        }

        if (!int.TryParse(args[1], out int damage)) {
            Console.Print("Invalid damage!");
            return;
        }

        if (!int.TryParse(args[2], out int delay)) {
            Console.Print("Invalid delay!");
            return;
        }

        if (!int.TryParse(args[3], out int duration)) {
            Console.Print("Invalid duration!");
            return;
        }

        _ = Helper.CreateComponent<TransientBehaviour>()
                  .Init(_ => player.DamagePlayerRpc(damage), duration, delay);
    }
}
