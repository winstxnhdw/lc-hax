using Hax;

[Command("ct")]
internal class ChibakuTenseiCommand : ICommand {
    Vector3 spinningY = new(0, 2, 0);

    Result TeleportPlayerToRandom(StringArray args) {
        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            return new Result(message: "Target player is not alive or found!");
        }

        Helper.BuyUnlockable(Unlockable.JACK_O_LANTERN);
        Helper.BuyUnlockable(Unlockable.ROMANTIC_TABLE);
        Helper.BuyUnlockable(Unlockable.RECORD_PLAYER);
        Helper.BuyUnlockable(Unlockable.TABLE);
        Helper.ReturnUnlockable(Unlockable.JACK_O_LANTERN);
        Helper.ReturnUnlockable(Unlockable.ROMANTIC_TABLE);
        Helper.ReturnUnlockable(Unlockable.RECORD_PLAYER);
        Helper.ReturnUnlockable(Unlockable.TABLE);

        if (Helper.GetUnlockable(Unlockable.CUPBOARD) is not PlaceableShipObject cupboard) {
            return new Result(message: "Cupboard not found!");
        }

        if (Helper.GetUnlockable(Unlockable.ROMANTIC_TABLE) is not PlaceableShipObject romanticTable) {
            return new Result(message: "Romantic table not found!");
        }

        if (Helper.GetUnlockable(Unlockable.JACK_O_LANTERN) is not PlaceableShipObject jackOLantern) {
            return new Result(message: "Jack O' Lantern not found!");
        }

        if (Helper.GetUnlockable(Unlockable.FILE_CABINET) is not PlaceableShipObject fileCabinet) {
            return new Result(message: "File cabinet not found!");
        }

        if (Helper.GetUnlockable(Unlockable.TABLE) is not PlaceableShipObject table) {
            return new Result(message: "Table not found!");
        }

        if (Helper.GetUnlockable(Unlockable.RECORD_PLAYER) is not PlaceableShipObject recordPlayer) {
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
            .Init(timeDelta => {
                distanceFromPlayerMultiplier = Mathf.Clamp(distanceFromPlayerMultiplier - (timeDelta * 3), 1, 5);
                closingInDirection = finalClosingIn * distanceFromPlayerMultiplier;
                changingTargetPlayerOffset.y += timeDelta * 0.1f;
                increasingSpiral += spiralPerSecond * timeDelta;
            }, duration - 3);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init(_ => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, cupboard, changingTargetPlayerOffset);
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init(_ => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, jackOLantern, changingTargetPlayerOffset + (Vector3.up * 4f));
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init(_ => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, romanticTable, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral, 0) * closingInDirection) + this.spinningY, Vector3.zero);
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init(_ => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, fileCabinet, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 90, 0) * closingInDirection) + this.spinningY, new Vector3(90, 0, 0));
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init(_ => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, table, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 180, 0) * closingInDirection) + this.spinningY, Vector3.zero);
            }, duration);

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init(_ => {
                Helper.PlaceObjectAtTransform(targetPlayer.transform, recordPlayer, changingTargetPlayerOffset + (Quaternion.Euler(0, increasingSpiral + 270, 0) * closingInDirection) + this.spinningY);
            }, duration);

        return new Result(true);
    }

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: ct <player>");
            return;
        }

        Result result = this.TeleportPlayerToRandom(args);

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
