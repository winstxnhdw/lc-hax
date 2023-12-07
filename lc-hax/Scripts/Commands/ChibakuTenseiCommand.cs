using System.Linq;
using UnityEngine;
using GameNetcodeStuff;
using Unity.Netcode;

namespace Hax;

public class ChibakuTenseiCommand : ICommand {
    private const int DURATION = 8;
    private Vector3 SPINNING_Y = new Vector3(0, 2, 0);

    Result TeleportPlayerToRandom(string[] args) {
        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.JACK_O_LANTERN);
        Helpers.BuyUnlockable(Unlockables.ROMANTIC_TABLE);
        Helpers.BuyUnlockable(Unlockables.TABLE);
        Helpers.BuyUnlockable(Unlockables.RECORD_PLAYER);
        PlaceableShipObject? romatic =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.ROMANTIC_TABLE);

        PlaceableShipObject? terminal =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.CUPBOARD);


        PlaceableShipObject? jack =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.JACK_O_LANTERN);


        PlaceableShipObject? cabinet =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.FILE_CABINET);

        PlaceableShipObject? table =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.TABLE);

        PlaceableShipObject? record =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.RECORD_PLAYER);

        if (romatic == null) {
            return new Result(message: "romatic not found!");
        }

        if (terminal == null) {
            return new Result(message: "terminal not found!");
        }

        Vector3 changingTargetPlayerOffset = Vector3.zero;
        float increasingSpiral = 0;
        float spiralPerSecond = 720;
        float distanceFromPlayerMultiplier = 5;
        Vector3 finalClosingIn = Vector3.forward * 1.25f;
        Vector3 closingInDirection = Vector3.forward;
        GameObject countDown = new();
        _ = countDown.AddComponent<TransientBehaviour>().Init(
            (timeDelta) => {
                distanceFromPlayerMultiplier = System.Math.Clamp(distanceFromPlayerMultiplier - timeDelta * 3, 1, 5);
                closingInDirection = finalClosingIn * distanceFromPlayerMultiplier;
                changingTargetPlayerOffset.y += timeDelta * 0.1f;
                increasingSpiral += spiralPerSecond * timeDelta;
            }, DURATION - 3);


        GameObject g2 = new();
        _ = g2.AddComponent<TransientBehaviour>().Init(
            (x) => {
                Helpers.PlaceObjectAtTransform(targetPlayer.transform, terminal, changingTargetPlayerOffset).Invoke(x);
            }, DURATION);

        GameObject g3 = new();
        _ = g3.AddComponent<TransientBehaviour>().Init(
            (x) => {
                Helpers.PlaceObjectAtTransform(targetPlayer.transform, jack, changingTargetPlayerOffset + Vector3.up * 4f).Invoke(x);
            }, DURATION);

        GameObject g1 = new();
        _ = g1.AddComponent<TransientBehaviour>().Init(
            (x) => {
                Helpers.PlaceObjectAtTransform(targetPlayer.transform, romatic, changingTargetPlayerOffset + Quaternion.Euler(0, increasingSpiral, 0) * closingInDirection + this.SPINNING_Y, Vector3.zero).Invoke(x);
            }, DURATION);

        GameObject g4 = new();
        _ = g4.AddComponent<TransientBehaviour>().Init(
            (x) => {
                Helpers.PlaceObjectAtTransform(targetPlayer.transform, cabinet, changingTargetPlayerOffset + Quaternion.Euler(0, increasingSpiral + 90, 0) * closingInDirection + this.SPINNING_Y, new Vector3(90, 0, 0)).Invoke(x);
            }, DURATION);

        GameObject g5 = new();
        _ = g5.AddComponent<TransientBehaviour>().Init(
            (x) => {
                Helpers.PlaceObjectAtTransform(targetPlayer.transform, table, changingTargetPlayerOffset + Quaternion.Euler(0, increasingSpiral + 180, 0) * closingInDirection + this.SPINNING_Y, Vector3.zero).Invoke(x);
            }, DURATION);

        GameObject g6 = new();
        _ = g6.AddComponent<TransientBehaviour>().Init(
            (x) => {
                Helpers.PlaceObjectAtTransform(
                    targetPlayer.transform,
                    record,
                    changingTargetPlayerOffset
                    + Quaternion.Euler(0, increasingSpiral + 270, 0)
                    * closingInDirection
                    + this.SPINNING_Y).Invoke(x);
            }, DURATION);



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
