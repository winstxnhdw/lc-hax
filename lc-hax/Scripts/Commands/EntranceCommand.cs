using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class EntranceCommand : ICommand {
    public void Execute(string[] args) {
        EntranceTeleport entranceTeleport = RoundManager.FindMainEntranceScript(true);

        if (!entranceTeleport.FindExitPoint()) {
            Console.Print("Exit point not found!");
            return;
        }

        if (!Reflector.Target(entranceTeleport).GetInternalField<Transform>("exitPoint").IsNotNull(out Transform exitPoint)) {
            Console.Print("Exit point not found!");
            return;
        }

        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Console.Print("Local player not found!");
            return;
        }

        localPlayer.TeleportPlayer(
            args.Length is not 0 && args[0] is "inside"
                ? exitPoint.position
                : entranceTeleport.entrancePoint.position
        );
    }
}
