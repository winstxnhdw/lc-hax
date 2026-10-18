using GameNetcodeStuff;
using UnityEngine;
using ZLinq;

sealed class AutoPlayInventory(PlayerControllerB player) {
    const float GrabDistance = 3.0f;

    PlayerControllerB Player { get; } = player;

    internal bool IsBusy =>
        this.Player is { isGrabbingObjectAnimation: true } or
                       { throwingObject: true } or
                       { activatingItem: true } or
                       { inSpecialInteractAnimation: true } or
                       { timeSinceSwitchingSlots: < 0.3f };

    internal bool ShouldReturn => this.Player.FirstEmptyItemSlot() == -1;

    internal bool HasScrap => this.Player.ItemSlots.AsValueEnumerable().Any(AutoPlayInventory.IsCargo);

    static bool IsCargo(GrabbableObject? item) =>
        item.Unfake() is { itemProperties.isScrap: true } and not StunGrenadeItem { explodeOnCollision: true };

    static bool IsAvailableScrap(GrabbableObject item) =>
        item.Unfake() is {
            IsSpawned: true,
            gameObject.activeInHierarchy: true,
            itemProperties.isScrap: true,
            grabbable: true,
            deactivated: false,
            isHeld: false,
            heldByPlayerOnServer: false,
            isHeldByEnemy: false,
            isPocketed: false,
            isInShipRoom: false,
            isInElevator: false
        };

    internal bool IsCollectible(GrabbableObject item) =>
        AutoPlayInventory.IsAvailableScrap(item) &&
        item is not (LungProp or StunGrenadeItem { explodeOnCollision: true }) &&
        !Helper.Enemies.AsValueEnumerable().Any(enemy => enemy.Unfake() is RedLocustBees { isEnemyDead: false } bees && bees.hive == item);

    internal Vector3 GetAimPosition(GrabbableObject item) => item.TryGetComponent(out Collider collider)
        ? collider.bounds.center
        : item.transform.position + (Vector3.up * 0.15f);

    internal bool TryGrab(GrabbableObject item) {
        Transform camera = this.Player.gameplayCamera.transform;
        float distance = Mathf.Min(AutoPlayInventory.GrabDistance, this.Player.grabDistance);
        int mask = this.Player.interactableObjectsMask | this.Player.playersManager.collidersAndRoomMaskAndDefault;
        bool hasHit = Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, distance, mask);

        if (!hasHit || hit.collider.GetComponent<GrabbableObject>() != item) {
            return false;
        }

        this.Player.BeginGrabObject();
        return this.Player.isGrabbingObjectAnimation;
    }

    internal void UnloadNext(StartOfRound round) {
        if (this.Player.currentlyHeldObjectServer.Unfake() is { } held && AutoPlayInventory.IsCargo(held)) {
            Vector3 dropOrigin = this.Player.cameraUp > 56.0f ? this.Player.gameplayCamera.transform.position : default;
            Vector3 floor = held.GetItemFloorPosition(dropOrigin);

            if (!round.shipInnerRoomBounds.bounds.Contains(floor + (Vector3.up * 0.25f))) {
                return;
            }

            this.Player.DiscardHeldObject();
            return;
        }

        for (int slot = 0; slot < this.Player.ItemSlots.Length; slot++) {
            if (!AutoPlayInventory.IsCargo(this.Player.ItemSlots[slot])) continue;

            this.Player.playerBodyAnimator.SetBool("GrabValidated", false);
            this.Player.SwitchToItemSlot(slot);
            this.Player.SwitchToSlotServerRpc(slot);
            this.Player.timeSinceSwitchingSlots = 0.0f;
            return;
        }
    }
}
