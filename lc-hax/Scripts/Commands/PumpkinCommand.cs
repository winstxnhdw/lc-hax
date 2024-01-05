using System;
using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

[Command("/pumpkin")]
public class PumpkinCommand : ICommand {
    bool PumpkinExists(float _) =>
        Helper.FindObjects<PlaceableShipObject>()
              .Where(placeableShipObject => placeableShipObject.unlockableID == (int)Unlockable.JACK_O_LANTERN)
              .Any();

    Action TeleportPumpkinToPlayerLater(PlayerControllerB player, float duration) => () => {
        if (!Helper.GetUnlockable(Unlockable.JACK_O_LANTERN).IsNotNull(out PlaceableShipObject jackOLantern)) return;

        Vector3 previousPosition = jackOLantern.transform.position.Copy();

        Helper.CreateComponent<TransientBehaviour>()
              .Init(_ => Helper.PlaceObjectAtTransform(player.playerGlobalHead.transform, jackOLantern), duration)
              .Dispose(() => {
                  Helper.PlaceObjectAtPosition(jackOLantern, previousPosition);
                  player.KillPlayer();
              });
    };

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /pumpkin <player> <duration=30>");
            return;
        }

        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.JACK_O_LANTERN);
        Helper.ReturnUnlockable(Unlockable.JACK_O_LANTERN);
        float duration = args.Length > 1 && float.TryParse(args[1], out float parsedDuration) ? parsedDuration : 15.0f;

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(this.PumpkinExists)
              .Init(this.TeleportPumpkinToPlayerLater(targetPlayer, duration));
    }
}
