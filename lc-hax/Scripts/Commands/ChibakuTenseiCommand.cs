using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class ChibakuTenseiCommand : ICommand {
    Vector3 spinningY = new(0, 2, 0);

    Result TeleportPlayerToRandom(string[] args) {
        if (!Helper.Extant(Helper.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helper.BuyUnlockable(Unlockable.JACK_O_LANTERN);
        Helper.BuyUnlockable(Unlockable.ROMANTIC_TABLE);
        Helper.BuyUnlockable(Unlockable.RECORD_PLAYER);
        Helper.BuyUnlockable(Unlockable.TABLE);

        if (!Helper.Extant(Helper.GetUnlockable(Unlockable.CUPBOARD), out PlaceableShipObject cupboard)) {
            return new Result(message: "Cupboard not found!");
        }

        if (!Helper.Extant(Helper.GetUnlockable(Unlockable.ROMANTIC_TABLE), out PlaceableShipObject romanticTable)) {
            return new Result(message: "Romantic table not found!");
        }

        if (!Helper.Extant(Helper.GetUnlockable(Unlockable.JACK_O_LANTERN), out PlaceableShipObject jackOLantern)) {
            return new Result(message: "Jack O' Lantern not found!");
        }

        if (!Helper.Extant(Helper.GetUnlockable(Unlockable.FILE_CABINET), out PlaceableShipObject fileCabinet)) {
            return new Result(message: "File cabinet not found!");
        }

        if (!Helper.Extant(Helper.GetUnlockable(Unlockable.TABLE), out PlaceableShipObject table)) {
            return new Result(message: "Table not found!");
        }

        if (!Helper.Extant(Helper.GetUnlockable(Unlockable.RECORD_PLAYER), out PlaceableShipObject recordPlayer)) {
            return new Result(message: "Cupboard not found!");
        }

        const int duration = 8;
        float increasingSpiral = 0;
        float spiralPerSecond = 720;
        float distanceFromPlayerMultiplier = 5;
        Vector3 changingTargetPlayerOffset = Vector3.zero;
        Vector3 finalClosingIn = Vector3.forward * 1.25f;
        Vector3 closingInDirection = Vector3.forward;

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init((timeDelta) => {
                distanceFromPlayerMultiplier = Mathf.Clamp(distanceFromPlayerMultiplier - (timeDelta * 3), 1, 5);
                closingInDirection = finalClosingIn * distanceFromPlayerMultiplier;
                changingTargetPlayerOffset.y += timeDelta * 0.1f;
                increasingSpiral += spiralPerSecond * timeDelta;
            }, duration - 3);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init((_) => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, cupboard, changingTargetPlayerOffset).Invoke(_);
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init((_) => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, jackOLantern, changingTargetPlayerOffset + (Vector3.up * 4f)).Invoke(_);
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init((_) => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, romanticTable, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral, 0) * closingInDirection) + this.spinningY, Vector3.zero).Invoke(_);
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init((_) => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, fileCabinet, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 90, 0) * closingInDirection) + this.spinningY, new Vector3(90, 0, 0)).Invoke(_);
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init((_) => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, table, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 180, 0) * closingInDirection) + this.spinningY, Vector3.zero).Invoke(_);
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init((_) => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, recordPlayer, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 270, 0) * closingInDirection) + this.spinningY).Invoke(_);
            }, duration);

        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Helper.PrintSystem("Usage: /ct <player>");
            return;
        }

        Result result = this.TeleportPlayerToRandom(args);

        if (!result.Success) {
            Helper.PrintSystem(result.Message);
        }
    }
}
