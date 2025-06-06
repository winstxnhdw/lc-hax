using GameNetcodeStuff;
using Unity.Netcode;

enum HoardingBugState {
    IDLE,
    SEARCHING_FOR_ITEMS,
    RETURNING_TO_NEST,
    CHASING_PLAYER,
    WATCHING_PLAYER,
    AT_NEST
}

class HoardingBugController : IEnemyController<HoarderBugAI> {
    static void UseHeldItem(HoarderBugAI enemy) {
        if (enemy.heldItem is not { itemGrabbableObject: GrabbableObject grabbable }) return;

        switch (grabbable) {
            case ShotgunItem gun:
                gun.ShootShotgun(enemy.transform);
                break;

            default:
                break;
        }
    }

    public void Update(HoarderBugAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (enemy.heldItem?.itemGrabbableObject is null) return;

        enemy.angryTimer = 0.0f;
        enemy.SetBehaviourState(HoardingBugState.IDLE);
    }

    static void GrabItem(HoarderBugAI enemy, GrabbableObject item) {
        if (!item.TryGetComponent(out NetworkObject networkObject)) return;

        enemy.sendingGrabOrDropRPC = true;
        enemy.GrabItem(networkObject);
        enemy.SwitchToBehaviourServerRpc(1);
        enemy.GrabItemServerRpc(networkObject);
    }

    public void OnDeath(HoarderBugAI enemy) {
        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) return;

        enemy.DropItemAndCallDropRPC(networkObject, false);
    }

    public void UsePrimarySkill(HoarderBugAI enemy) {
        if (enemy.angryTimer > 0.0f) {
            enemy.angryTimer = 0.0f;
            enemy.angryAtPlayer = null;
            enemy.SetBehaviourState(HoardingBugState.IDLE);
        }

        if (enemy.heldItem is null && enemy.FindNearbyItem() is GrabbableObject grabbable) {
            HoardingBugController.GrabItem(enemy, grabbable);
        }

        else {
            HoardingBugController.UseHeldItem(enemy);
        }
    }

    public void UseSecondarySkill(HoarderBugAI enemy) {
        if (enemy.heldItem?.itemGrabbableObject is null) {
            PlayerControllerB hostPlayer = Helper.Players[0];
            enemy.watchingPlayer = hostPlayer;
            enemy.angryAtPlayer = hostPlayer;
            enemy.angryTimer = 15.0f;
            enemy.SetBehaviourState(HoardingBugState.CHASING_PLAYER);
            return;
        }

        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) {
            enemy.DropItemAndCallDropRPC(networkObject, false);
        }
    }

    public string GetPrimarySkillName(HoarderBugAI enemy) => enemy.heldItem is not null ? "Use item" : "Grab Item";

    public string GetSecondarySkillName(HoarderBugAI enemy) => enemy.heldItem is null ? "" : "Drop item";

    public float InteractRange(HoarderBugAI _) => 1.0f;
}
