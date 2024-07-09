#region

using Hax;
using UnityEngine;

#endregion

public enum BushWolfState {
    Idle,
    Attacking,
    Dodging,
    Hiding,
    MatingCall,
    Animating,
    Following
}


class BushWolfController : IEnemyController<BushWolfEnemy> {
    readonly Vector3 camOffset = new(0, 3.2f, -4f);

    public Vector3 GetCameraOffset(BushWolfEnemy enemy) => this.camOffset;

    public void UsePrimarySkill(BushWolfEnemy enemy) =>
        enemy.SetBehaviourState(enemy.IsBehaviourState(BushWolfState.Following) ? BushWolfState.Hiding : BushWolfState.Following);

    public void UseSecondarySkill(BushWolfEnemy enemy) => enemy.SyncTargetPlayerAndAttackServerRpc(enemy.FindClosestPlayer().GetPlayerId());

    public string GetPrimarySkillName(BushWolfEnemy enemy) => enemy.IsBehaviourState(BushWolfState.Following) ? "Hide" : "Follow";

    public string GetSecondarySkillName(BushWolfEnemy _) => "Attack";


    public bool CanUseEntranceDoors(BushWolfEnemy enemy) => true;

    public string? GetSpecialAbilityName(BushWolfEnemy enemy) => "Mating Call";

    public void UseSpecialAbility(BushWolfEnemy enemy) => enemy.MatingCallServerRpc();
}
