using System.Linq;
using UnityEngine;
using GameNetcodeStuff;
using Unity.Netcode;

namespace Hax;

public class ChibakuTenseiCommand : ICommand {

    Result TeleportPlayerToRandom(string[] args) {
        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.ROMANTIC_TABLE);
        PlaceableShipObject? table =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.ROMANTIC_TABLE);

        PlaceableShipObject? terminal =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.TERMINAL);

        if (table == null) {
            return new Result(message: "table not found!");
        }

        if (terminal == null) {
            return new Result(message: "terminal not found!");
        }


        GameObject ogTable = new();
        ogTable.transform.position = Helpers.CopyVector(table.transform.position);
        ogTable.transform.eulerAngles = Helpers.CopyVector(table.transform.eulerAngles);
        GameObject ogTerminal = new();
        ogTerminal.transform.position = Helpers.CopyVector(terminal.transform.position);
        ogTerminal.transform.eulerAngles = Helpers.CopyVector(terminal.transform.eulerAngles);

        GameObject g1 = new();
        _ = g1.AddComponent<TransientObject>().Init(
            Helpers.PlaceObjectAtTransform(targetPlayer.transform, table, Vector3.zero, Vector3.zero), 5f)
            .Dispose(() => Helpers.PlaceObjectAtTransform(ogTable.transform, table).Invoke(0));

        GameObject g2 = new();
        _ = g2.AddComponent<TransientObject>().Init(
            Helpers.PlaceObjectAtTransform(targetPlayer.transform, terminal, Vector3.up * 5, Vector3.zero), 5f)
            .Dispose(() =>
                Helpers.PlaceObjectAtTransform(ogTerminal.transform, terminal).Invoke(0)
            );
        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /chibaku <player>");
            return;
        }

        Result result = this.TeleportPlayerToRandom(args);

        if (!result.Success) {
            Console.Print("SYSTEM", result.Message);
        }
    }
}
