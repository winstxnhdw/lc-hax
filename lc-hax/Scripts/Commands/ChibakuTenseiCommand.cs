using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class ChibakuTenseiCommand : ICommand {
    private const int duration = 8;
    private Vector3 spinningY = new(0, 2, 0);

    Result TeleportPlayerToRandom(string[] args) {
        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.JACK_O_LANTERN);
        Helpers.BuyUnlockable(Unlockables.ROMANTIC_TABLE);
        Helpers.BuyUnlockable(Unlockables.TABLE);
        Helpers.BuyUnlockable(Unlockables.RECORD_PLAYER);

        if (!Helpers.Extant(Helpers.GetUnlockable(Unlockables.CUPBOARD), out PlaceableShipObject cupboard)) {
            return new Result(message: "Cupboard not found!");
        }

        if (!Helpers.Extant(Helpers.GetUnlockable(Unlockables.ROMANTIC_TABLE), out PlaceableShipObject romanticTable)) {
            return new Result(message: "Romantic table not found!");
        }

        if (!Helpers.Extant(Helpers.GetUnlockable(Unlockables.JACK_O_LANTERN), out PlaceableShipObject jackOLantern)) {
            return new Result(message: "Jack O' Lantern not found!");
        }

        if (!Helpers.Extant(Helpers.GetUnlockable(Unlockables.FILE_CABINET), out PlaceableShipObject fileCabinet)) {
            return new Result(message: "File cabinet not found!");
        }

        if (!Helpers.Extant(Helpers.GetUnlockable(Unlockables.TABLE), out PlaceableShipObject table)) {
            return new Result(message: "Table not found!");
        }

        if (!Helpers.Extant(Helpers.GetUnlockable(Unlockables.RECORD_PLAYER), out PlaceableShipObject recordPlayer)) {
            return new Result(message: "Cupboard not found!");
        }

        float increasingSpiral = 0;
        float spiralPerSecond = 720;
        float distanceFromPlayerMultiplier = 5;
        Vector3 changingTargetPlayerOffset = Vector3.zero;
        Vector3 finalClosingIn = Vector3.forward * 1.25f;
        Vector3 closingInDirection = Vector3.forward;

        _ = new GameObject().AddComponent<TransientBehaviour>().Init(
            (timeDelta) => {
                distanceFromPlayerMultiplier = System.Math.Clamp(distanceFromPlayerMultiplier - (timeDelta * 3), 1, 5);
                closingInDirection = finalClosingIn * distanceFromPlayerMultiplier;
                changingTargetPlayerOffset.y += timeDelta * 0.1f;
                increasingSpiral += spiralPerSecond * timeDelta;
            }, duration - 3);

        _ = new GameObject()
            .AddComponent<TransientBehaviour>()
            .Init((_) => Helpers.PlaceObjectAtTransform(targetPlayer.transform, cupboard, changingTargetPlayerOffset).Invoke(_), duration);

        _ = new GameObject()
            .AddComponent<TransientBehaviour>()
            .Init((_) => Helpers.PlaceObjectAtTransform(targetPlayer.transform, jackOLantern, changingTargetPlayerOffset + (Vector3.up * 4f)).Invoke(_), duration);

        _ = new GameObject()
            .AddComponent<TransientBehaviour>()
            .Init((_) => Helpers.PlaceObjectAtTransform(targetPlayer.transform, romanticTable, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral, 0) * closingInDirection) + this.spinningY, Vector3.zero).Invoke(_), duration);

        _ = new GameObject()
            .AddComponent<TransientBehaviour>()
            .Init((_) => Helpers.PlaceObjectAtTransform(targetPlayer.transform, fileCabinet, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 90, 0) * closingInDirection) + this.spinningY, new Vector3(90, 0, 0)).Invoke(_), duration);

        _ = new GameObject()
            .AddComponent<TransientBehaviour>()
            .Init((_) => Helpers.PlaceObjectAtTransform(targetPlayer.transform, table, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 180, 0) * closingInDirection) + this.spinningY, Vector3.zero).Invoke(_), duration);

        _ = new GameObject()
            .AddComponent<TransientBehaviour>()
            .Init((_) => Helpers.PlaceObjectAtTransform(targetPlayer.transform, recordPlayer, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 270, 0) * closingInDirection) + this.spinningY).Invoke(_), duration);

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
