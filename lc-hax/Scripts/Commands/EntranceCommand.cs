using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class EntranceCommand : ICommand {
    public bool forceInside = false;

    public EntranceCommand(bool inside = false) {
        this.forceInside = inside;
    }

    public void Execute(string[] args) {
        EntranceTeleport entranceTeleport = RoundManager.FindMainEntranceScript(true);

        if (!entranceTeleport.FindExitPoint()) {
            Console.Print("Exit point not found!");
            return;
        }

        if (!entranceTeleport.Reflect().GetInternalField<Transform>("exitPoint").IsNotNull(out Transform exitPoint)) {
            Console.Print("Exit point not found!");
            return;
        }

        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Console.Print("Local player not found!");
            return;
        }

        localPlayer.TeleportPlayer(
            this.forceInside || (args.Length is not 0 && args[0] is "inside")
                ? exitPoint.position
                : entranceTeleport.entrancePoint.position
        );
    }
}
