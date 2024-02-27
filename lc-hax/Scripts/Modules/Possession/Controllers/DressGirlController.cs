using GameNetcodeStuff;
using Hax;
using Unity.Netcode;
using UnityEngine;

enum DressGirlState {
    PEACAFUL,
    HOSTEL,
}

internal class DressGirlController : IEnemyController<DressGirlAI> {

    bool IsVisible { get; set; } = false;
    bool IsChasing { get; set; } = false;
    bool IsStalking { get; set; } = false;

    public void UsePrimarySkill(DressGirlAI enemy) {
        if (IsChasing) return;
        IsVisible = true;
        IsChasing = true;
    }

    public void UseSecondarySkill(DressGirlAI enemy) {
        IsVisible = true;
        IsStalking = true;
    }

    public void ReleaseSecondarySkill(DressGirlAI enemy) {
        IsVisible = false;
        IsChasing = false;
        IsStalking = false;
    }

    public void OnMovement(DressGirlAI enemy, bool isMoving, bool isSprinting) {
        if (!IsVisible) return;
        if (isMoving) {
        }
        else {
        }
    }

    public void Update(DressGirlAI enemy, bool isAIControlled) {
        PlayerControllerB hostPlayer = Helper.Players[0]; // For Testing
        enemy.hauntingPlayer = hostPlayer;

        if (IsChasing) {
            enemy.SetBehaviourState(DressGirlState.HOSTEL);
        }
        else {
            enemy.SetBehaviourState(DressGirlState.PEACAFUL);
        }

        if (IsVisible) {
            ClientSideIsVisible(enemy);
        }
        else {
            ClientSideIsNotVisible(enemy);
        }
    }

    internal static void SendUITip(string header, string message, bool warning) {
        HUDManager.Instance?.DisplayTip(header, message, warning, false, "LC_Tip1");
    }

    public void OnPossess(DressGirlAI enemy) => ClientSideIsNotVisible(enemy);

    public void OnUnpossess(DressGirlAI enemy) {
        ReleaseSecondarySkill(enemy);
        ClientSetInvisible(enemy);
    }

    void ClientSideIsVisible(DressGirlAI enemy) {
        for (int Body = 0; Body < enemy.skinnedMeshRenderers.Length; Body++) {
            enemy.skinnedMeshRenderers[Body].gameObject.layer = 19;
            enemy.skinnedMeshRenderers[Body].material.color = new Color(1f, 1f, 1f);
        }
        for (int Eyes = 0; Eyes < enemy.meshRenderers.Length; Eyes++) {
            enemy.meshRenderers[Eyes].gameObject.layer = 19;
            enemy.meshRenderers[Eyes].material.color = new Color(1f, 1f, 1f);
        }
    }

    void ClientSideIsNotVisible(DressGirlAI enemy) {
        for (int Body = 0; Body < enemy.skinnedMeshRenderers.Length; Body++) {
            enemy.skinnedMeshRenderers[Body].gameObject.layer = 19;
            enemy.skinnedMeshRenderers[Body].material.color = new Color(0f, 0f, 0f);
        }
        for (int Eyes = 0; Eyes < enemy.meshRenderers.Length; Eyes++) {
            enemy.meshRenderers[Eyes].gameObject.layer = 19;
            enemy.meshRenderers[Eyes].material.color = new Color(0f, 0f, 0f);
        }
    }

    void ClientSetInvisible(DressGirlAI enemy) {
        for (int Body = 0; Body < enemy.skinnedMeshRenderers.Length; Body++) {
            enemy.skinnedMeshRenderers[Body].gameObject.layer = 23;
            enemy.skinnedMeshRenderers[Body].material.color = new Color(1f, 1f, 1f);
        }
        for (int Eyes = 0; Eyes < enemy.meshRenderers.Length; Eyes++) {
            enemy.meshRenderers[Eyes].gameObject.layer = 23;
            enemy.meshRenderers[Eyes].material.color = new Color(1f, 1f, 1f);
        }
    }

    public string GetPrimarySkillName(DressGirlAI enemy) => enemy.IsBehaviourState(DressGirlState.HOSTEL) ? "" : "Chase";

    public string GetSecondarySkillName(DressGirlAI enemy) => enemy.IsBehaviourState(DressGirlState.HOSTEL) ? "Stop Chasing" : "(HOLD) Show";

    public bool CanUseEntranceDoors(DressGirlAI _) => true;

    public float InteractRange(DressGirlAI _) => 1.0f;

    public bool SyncAnimationSpeedEnabled(DressGirlAI _) => true;
}
