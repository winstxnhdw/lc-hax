using GameNetcodeStuff;
using Hax;
using UnityEngine;

public enum RadMechBehaviorState {
    Default,
    Alert,
    Flying,
}
internal class RadMechController : IEnemyController<RadMechAI> {

    Vector3 camOffset = new(0, 8f, -8f);

    public Vector3 GetCameraOffset(RadMechAI enemy) => this.camOffset;

    public bool IsAbleToMove(RadMechAI enemy) => !enemy.inTorchPlayerAnimation;

    public bool CanUseEntranceDoors(RadMechAI _) => false;

    public float InteractRange(RadMechAI _) => 0f;

    public bool SyncAnimationSpeedEnabled(RadMechAI _) => true;

    public void OnOutsideStatusChange(RadMechAI enemy) => enemy.StopSearch(enemy.searchForPlayers, true);

    public void UsePrimarySkill(RadMechAI enemy) {
        enemy.SetBehaviourState(RadMechBehaviorState.Alert);
        PlayerControllerB player = enemy.FindClosestPlayer(4f);
        enemy.targetPlayer = player;
        enemy.targetedThreat = player.ToThreat();
    }

    public void UseSecondarySkill(RadMechAI enemy) {
        bool inFlyingMode = enemy.Reflect().GetInternalField<bool>("inFlyingMode");
        if (!inFlyingMode) {
            enemy.SetBehaviourState(RadMechBehaviorState.Flying);
            enemy.targetPlayer = enemy.FindClosestPlayer(4f);
            enemy.StartFlying();
        }
        else {
            enemy.EndFlight();
        }
    }






}
