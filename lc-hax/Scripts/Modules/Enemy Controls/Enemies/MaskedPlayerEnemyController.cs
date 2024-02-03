using UnityEngine;

public static class MaskedPlayerController {

    public static void Crouch(this MaskedPlayerEnemy instance, bool crouching) {
        if (instance == null) return;
        instance.SetCrouchingServerRpc(crouching);
    }

    public static void ScanNearbyAndOpenMainEntranceDoor(this MaskedPlayerEnemy instance, float entranceDoorOpenRange = 2.5f) {
        Collider[] array = Physics.OverlapSphere(instance.transform.position, entranceDoorOpenRange);
        for (int i = 0; i < array.Length; i++)
            if (array[i].TryGetComponent(out EntranceTeleport door))
                if (door.entranceId == 0)
                    instance.UseEntranceDoor();
    }

    public static void UseEntranceDoor(this MaskedPlayerEnemy instance) {
        Vector3 vector = RoundManager.FindMainEntrancePosition(true, !instance.isOutside);
        instance.transform.position = vector;
        _ = instance.Reflect().InvokeInternalMethod("TeleportMaskedEnemyAndSync",
        [
                RoundManager.FindMainEntrancePosition(true, !instance.isOutside),
            !instance.isOutside
        ]);
        canUseEntranceDoorOnCollision = false;
    }

    public static void OnControllerColliderHit(this MaskedPlayerEnemy instance, ControllerColliderHit hit) {
        if (canUseEntranceDoorOnCollision)
            if (hit.gameObject.TryGetComponent(out EntranceTeleport door))
                if (door.entranceId == 0)
                    instance.UseEntranceDoor();
    }

    public static void UpdateEvent(this MaskedPlayerEnemy instance) {
        if (instance == null) return;

        if (!canUseEntranceDoorOnCollision) {
            timeSinceLastEntranceDoorUse += Time.deltaTime;
            if (timeSinceLastEntranceDoorUse >= useEntranceDoorCooldown) {
                canUseEntranceDoorOnCollision = true;
                timeSinceLastEntranceDoorUse = 0f;
            }
        }
    }

    public static void ResetCooldown(this MaskedPlayerEnemy _) {
        useEntranceDoorCooldown = 2f;
        timeSinceLastEntranceDoorUse = 0f;
        canUseEntranceDoorOnCollision = true;
    }

    public static float useEntranceDoorCooldown = 2f;
    public static float timeSinceLastEntranceDoorUse = 0f;

    public static bool canUseEntranceDoorOnCollision = true;
}
