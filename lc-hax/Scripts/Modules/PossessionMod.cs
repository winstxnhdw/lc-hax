using System;
using UnityEngine;
using System.Collections;
using GameNetcodeStuff;
using UnityEngine.AI;
using Hax;
using UnityEngine.UI;

public sealed class PossessionMod : MonoBehaviour {
    // Singleton instance of the PossessionMod
    public static PossessionMod? Instance { get; private set; }

    // Nullable property representing the enemy to possess
    EnemyAI? EnemyToPossess { get; set; } = null;

    // Flag to determine if it's the first update after possession
    bool FirstUpdate { get; set; } = true;

    // Nullable properties for various movement components
    RigidbodyMovement? RigidbodyKeyboard { get; set; } = null;
    KeyboardMovement? Keyboard { get; set; } = null;
    MousePan? MousePan { get; set; } = null;

    EnemyIdentity enemyIdentity = EnemyIdentity.None;

    // Readonly property indicating if possessing an enemy
    public bool IsPossessed => this.EnemyToPossess != null;

    public bool isUsingPrimarySkill { get; set; } = false;

    public bool isUsingSecondarySkill { get; set; } = false;

    // Flag for no clipping mode
    bool NoClipEnabled { get; set; } = false;

    // Initializes movement components and disables the script initially
    void Awake() {
        this.RigidbodyKeyboard = this.gameObject.AddComponent<RigidbodyMovement>();
        this.Keyboard = this.gameObject.AddComponent<KeyboardMovement>();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        this.enabled = false;

        PossessionMod.Instance = this;
    }

    // Subscribes to input events and updates components based on the current state
    void OnEnable() {
        InputListener.onNPress += this.ToggleNoClip;
        InputListener.onXPress += this.ToggleRealisticPossession;
        InputListener.onZPress += this.Unpossess;
        InputListener.onLeftButtonPress  += this.UsePrimarySkill;
        InputListener.onLeftButtonRelease += this.ReleasePrimarySkill;
        InputListener.onRightButtonPress += this.UseSecondarySkill;
        InputListener.onRightButtonRelease += this.ReleaseSecondarySkill;

        this.UpdateComponentsOnCurrentState(true);
    }

    // Unsubscribes from input events
    void OnDisable() {
        InputListener.onNPress -= this.ToggleNoClip;
        InputListener.onXPress -= this.ToggleRealisticPossession;
        InputListener.onZPress -= this.Unpossess;

        this.UpdateComponentsOnCurrentState(false);
    }

    // Toggles realistic possession mode
    private void ToggleRealisticPossession() {
        Setting.RealisticPossessionEnabled = !Setting.RealisticPossessionEnabled;
        Chat.Print($"Realistic Possession: {Setting.RealisticPossessionEnabled}");

        if (this.EnemyToPossess?.agent.Unfake() is not NavMeshAgent navMeshAgent) {
            return;
        }

        navMeshAgent.updatePosition = Setting.RealisticPossessionEnabled;
        navMeshAgent.updateRotation = Setting.RealisticPossessionEnabled;
    }

    // Toggles no clipping mode
    private void ToggleNoClip() {
        this.NoClipEnabled = !this.NoClipEnabled;
        Chat.Print($"Possess NoClip: {this.NoClipEnabled}");

        this.UpdateComponentsOnCurrentState(this.enabled);
    }

    // Updates movement components based on the current state
    private void UpdateComponentsOnCurrentState(bool thisGameObjectIsEnabled) {
        if (this.MousePan is not MousePan mousePan) {
            return;
        }

        if (this.RigidbodyKeyboard is not RigidbodyMovement rigidbodyKeyboard) {
            return;
        }

        if (this.Keyboard is not KeyboardMovement keyboard) {
            return;
        }

        mousePan.enabled = thisGameObjectIsEnabled;
        rigidbodyKeyboard.enabled = !this.NoClipEnabled;
        keyboard.enabled = this.NoClipEnabled;
    }

    // Updates position and rotation of possessed enemy
    void Update() {
        _ = this.StartCoroutine(this.EndOfFrameCoroutine());
    }

    // Coroutine for updating at end of frame
    IEnumerator EndOfFrameCoroutine() {
        yield return new WaitForEndOfFrame();
        this.EndOfFrameUpdate();
    }

    private void HandleEnemyMovements() {
        
        if (this.EnemyToPossess is null) return;
        switch (this.enemyIdentity) {
            case EnemyIdentity.Nutcracker:
                if(this.isUsingPrimarySkill || this.isUsingSecondarySkill) return; // prevent this from blocking the sentry skill of the nutcracker.
                ((NutcrackerEnemyAI)this.EnemyToPossess).OnMoving();
                break;
            default:
                break;
        }
    }
    private void EnemyUpdate() {
        if (this.EnemyToPossess is null) return;
        switch (this.enemyIdentity) {
            case EnemyIdentity.Masked:
                ((MaskedPlayerEnemy)this.EnemyToPossess).Update();
                break;
            default:
                break;
        }
    }

    private bool CanMoveEnemy() {
        if (this.EnemyToPossess is null) return true;
        return this.enemyIdentity switch {
            EnemyIdentity.Centipede => ((CentipedeAI)this.EnemyToPossess).CanMove(),
            EnemyIdentity.Flowerman => ((FlowermanAI)this.EnemyToPossess).CanMove(),
            EnemyIdentity.ForestGiant => ((ForestGiantAI)this.EnemyToPossess).CanMove(),
            EnemyIdentity.Jester => ((JesterAI)this.EnemyToPossess).CanMove(),
            _ => true,
        };
    }

    // Updates position and rotation of possessed enemy at the end of frame
    private void EndOfFrameUpdate() {
        if (this.RigidbodyKeyboard is not RigidbodyMovement rigidbodyKeyboard) return;
        if (this.EnemyToPossess is not EnemyAI enemy) return;
        if (Helper.CurrentCamera is not Camera camera || !camera.enabled) return;

        if (this.FirstUpdate) {
            // Save enemy's position before possession
            this.SetEnemyColliders(enemy, false);

            if (enemy.agent.Unfake() is NavMeshAgent agent) {
                agent.updatePosition = Setting.RealisticPossessionEnabled;
                agent.updateRotation = Setting.RealisticPossessionEnabled;
            }

            rigidbodyKeyboard.Init();
            this.transform.position = enemy.transform.position;
            this.UpdateComponentsOnCurrentState(true);
        }
        this.UpdateEnemyPositionToHere(enemy);
        this.EnemyUpdate();
        camera.transform.position = this.transform.position + (Vector3.up * 3f) - (enemy.transform.forward * 3f);
        camera.transform.rotation = this.transform.rotation;

        this.FirstUpdate = false;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit) {
        if (this.EnemyToPossess is null) return;
        if (this.enemyIdentity == EnemyIdentity.Masked) ((MaskedPlayerEnemy)this.EnemyToPossess).OnControllerColliderHit(hit);
    }

    // Updates enemy's position to match the possessed object's position
    void UpdateEnemyPositionToHere(EnemyAI enemy) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (!this.CanMoveEnemy()) return;
        this.HandleEnemyMovements();
        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        enemy.updatePositionThreshold = 0;
        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;
        enemy.transform.eulerAngles = enemyEuler;
        enemy.transform.position = this.transform.position;
    }

    // Disables/enables colliders of the possessed enemy
    void SetEnemyColliders(EnemyAI enemy, bool enabled) {
        enemy.GetComponentsInChildren<Collider>().ForEach(collider => collider.enabled = enabled);
    }

    // Possesses the specified enemy
    public void Possess(EnemyAI enemy) {
        this.Unpossess();

        this.EnemyToPossess = enemy;
        this.FirstUpdate = true;
        this.enemyIdentity = this.IdentifyEnemy(enemy);
        Chat.Print($"Possessing {this.enemyIdentity}");

    }

    // Releases possession of the current enemy
    public void Unpossess() {
        if (this.EnemyToPossess is EnemyAI previousEnemy) {
            previousEnemy.updatePositionThreshold = 1;

            if (previousEnemy.agent.Unfake() is NavMeshAgent navMeshAgent) {
                navMeshAgent.updatePosition = true;
                navMeshAgent.updateRotation = true;
                this.UpdateEnemyPositionToHere(previousEnemy);
                _ = previousEnemy.agent.Warp(previousEnemy.transform.position);
            }

            this.SetEnemyColliders(previousEnemy, true);
        }

        this.EnemyToPossess = null;
        this.enemyIdentity = EnemyIdentity.None;
        this.isUsingPrimarySkill = false;
        this.isUsingSecondarySkill = false;
    }

    public void UsePrimarySkill() {
        if (this.EnemyToPossess is null) return;
        this.isUsingPrimarySkill = true;
        switch (this.enemyIdentity) {
            case EnemyIdentity.Centipede:
                ((CentipedeAI)this.EnemyToPossess).UsePrimarySkill();
                break;
            case EnemyIdentity.Flowerman:
                ((FlowermanAI)this.EnemyToPossess).UsePrimarySkill();
                break;
            case EnemyIdentity.HoarderBug:
                ((HoarderBugAI)this.EnemyToPossess).UsePrimarySkill();
                break;
            case EnemyIdentity.Jester:
                ((JesterAI)this.EnemyToPossess).UsePrimarySkill();
                break;
            case EnemyIdentity.Nutcracker:
                ((NutcrackerEnemyAI)this.EnemyToPossess).UsePrimarySkill();
                break;
            case EnemyIdentity.Puffer:
                ((PufferAI)this.EnemyToPossess).UsePrimarySkill();
                break;
            default:
                break;
        }
    }

    public void UseSecondarySkill() {
        if (this.EnemyToPossess is null) return;
        this.isUsingSecondarySkill = true;
        switch (this.enemyIdentity) {
            case EnemyIdentity.Centipede:
                ((CentipedeAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.Baboon:
                ((BaboonBirdAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.Flowerman:
                ((FlowermanAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.ForestGiant:
                ((ForestGiantAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.HoarderBug:
                ((HoarderBugAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.Jester:
                ((JesterAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.MouthDog:
                ((MouthDogAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.Nutcracker:
                ((NutcrackerEnemyAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.Puffer:
                ((PufferAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            case EnemyIdentity.Sandworm:
                ((SandWormAI)this.EnemyToPossess).UseSecondarySkill();
                break;
            default:
                break;
        }
    }

    public void ReleasePrimarySkill() {
        if (this.EnemyToPossess is null) return;
        this.isUsingPrimarySkill = false;
    }
    public void ReleaseSecondarySkill() {
        if (this.EnemyToPossess is null) return;

        switch (this.enemyIdentity) {
            case EnemyIdentity.Flowerman:
                ((FlowermanAI)this.EnemyToPossess).ReleaseSecondarySkill();
                break;
            case EnemyIdentity.ForestGiant:
                ((ForestGiantAI)this.EnemyToPossess).ReleaseSecondarySkill();
                break;
            case EnemyIdentity.Jester:
                ((JesterAI)this.EnemyToPossess).ReleaseSecondarySkill();
                break;
            case EnemyIdentity.Nutcracker:
                ((NutcrackerEnemyAI)this.EnemyToPossess).ReleaseSecondarySkill();
                break;
            default:
                break;
        }
    }

    public string GetPrimarySkillName() {
        if (this.EnemyToPossess is null) return "";

        switch (this.enemyIdentity) {
            case EnemyIdentity.Centipede:
                return ((CentipedeAI)this.EnemyToPossess).GetPrimarySkillName();
            case EnemyIdentity.Flowerman:
                return ((FlowermanAI)this.EnemyToPossess).GetPrimarySkillName();
            case EnemyIdentity.HoarderBug:
                return ((HoarderBugAI)this.EnemyToPossess).GetPrimarySkillName();
            case EnemyIdentity.Jester:
                return ((JesterAI)this.EnemyToPossess).GetPrimarySkillName();
            case EnemyIdentity.Nutcracker:
                return ((NutcrackerEnemyAI)this.EnemyToPossess).GetPrimarySkillName();
            case EnemyIdentity.Puffer:
                return ((PufferAI)this.EnemyToPossess).GetPrimarySkillName();
            default:
                return "";
        }
    }

    public string GetSecondarySkillName() {
        if (this.EnemyToPossess is null) return "";

        switch (this.enemyIdentity) {
            case EnemyIdentity.Centipede:
                return ((CentipedeAI)this.EnemyToPossess).GetSecondarySkillName();
            case EnemyIdentity.Baboon:
                return ((BaboonBirdAI)this.EnemyToPossess).GetSecondarySkillName();
            case EnemyIdentity.HoarderBug:
                return ((HoarderBugAI)this.EnemyToPossess).GetSecondarySkillName();
            case EnemyIdentity.Jester:
                return ((JesterAI)this.EnemyToPossess).GetSecondarySkillName();
            case EnemyIdentity.Nutcracker:
                return ((NutcrackerEnemyAI)this.EnemyToPossess).GetSecondarySkillName();
            case EnemyIdentity.Puffer:
                return ((PufferAI)this.EnemyToPossess).GetSecondarySkillName();
            case EnemyIdentity.Sandworm:
                return ((SandWormAI)this.EnemyToPossess).GetSecondarySkillName();
            case EnemyIdentity.MouthDog:
                return ((MouthDogAI)this.EnemyToPossess).GetSecondarySkillName();

            default:
                return "";
        }
    }


    public EnemyIdentity IdentifyEnemy(EnemyAI enemy) {
        if (enemy is null) return EnemyIdentity.None;
        return enemy switch {
            CentipedeAI => EnemyIdentity.Centipede,
            BaboonBirdAI => EnemyIdentity.Baboon,
            FlowermanAI => EnemyIdentity.Flowerman,
            ForestGiantAI => EnemyIdentity.ForestGiant,
            HoarderBugAI => EnemyIdentity.HoarderBug,
            JesterAI => EnemyIdentity.Jester,
            MaskedPlayerEnemy => EnemyIdentity.Masked,
            MouthDogAI => EnemyIdentity.MouthDog,
            NutcrackerEnemyAI => EnemyIdentity.Nutcracker,
            PufferAI => EnemyIdentity.Puffer,
            SandWormAI => EnemyIdentity.Sandworm,
            SpringManAI => EnemyIdentity.Springman,
            _ => EnemyIdentity.Default,
        };
    }

}
