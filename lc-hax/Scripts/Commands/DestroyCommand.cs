using System.Linq;
using GameNetcodeStuff;
using Hax;

[Command("/destroy")]
internal class DestroyCommand : ICommand {
    Result DestroyHeldItem(PlayerControllerB player) {
        if (player.currentlyHeldObjectServer is null) {
            return new Result(message: "You are not holding anything!");
        }

        player.DespawnHeldObject();
        return new Result(true);
    }

    Result DestroyAllItems(PlayerControllerB player) {
        Helper.Grabbables.ToArray().ForEach(grabbable => {
            player.currentlyHeldObjectServer = grabbable;
            player.DespawnHeldObject();
        });

        return new Result(true);
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        Result result = args[0] switch {
            null => this.DestroyHeldItem(player),
            "--all" => this.DestroyAllItems(player),
            _ => new Result(message: "Invalid arguments!")
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
