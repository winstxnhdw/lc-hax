using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameNetcodeStuff;
using Hax;

internal sealed class PossessionMod : MonoBehaviour {
    internal static PossessionMod? Instance { get; private set; }
    internal bool IsPossessed => this.EnemyToPossess is not null;

    EnemyAI? EnemyToPossess { get; set; } = null;
    Coroutine? UpdateCoroutine { get; set; } = null;
    RigidbodyMovement? RigidbodyKeyboard { get; set; } = null;
    KeyboardMovement? Keyboard { get; set; } = null;
    MousePan? MousePan { get; set; } = null;

    bool FirstUpdate { get; set; } = true;
    bool NoClipEnabled { get; set; } = false;

    Dictionary<Type, IController> EnemyControllers { get; } = new() {
        { typeof(CentipedeAI), new SnareFleaController() },
        { typeof(FlowermanAI), new BrackenController() },
        { typeof(ForestGiantAI), new ForestGiantController() },
        { typeof(HoarderBugAI), new HoardingBugController() },
        { typeof(JesterAI), new JesterController() },
        { typeof(NutcrackerEnemyAI), new NutcrackerController() },
        { typeof(PufferAI), new SporeLizardController() },
        { typeof(BaboonBirdAI), new BaboonHawkController() },
        { typeof(SandWormAI), new EarthLeviathanController() },
        { typeof(MouthDogAI), new EyelessDogController() },
        { typeof(MaskedPlayerEnemy), new MaskedPlayerController() }
    };

    void Awake() {
        this.RigidbodyKeyboard = this.gameObject.AddComponent<RigidbodyMovement>();
        this.Keyboard = this.gameObject.AddComponent<KeyboardMovement>();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        this.enabled = false;

        PossessionMod.Instance = this;
    }

    void OnEnable() {
        InputListener.OnNPress += this.ToggleNoClip;
        InputListener.OnXPress += this.ToggleRealisticPossession;
        InputListener.OnZPress += this.Unpossess;
        InputListener.OnLeftButtonPress += this.UsePrimarySkill;
        InputListener.OnRightButtonHold += this.OnRightMouseButtonHold;

        this.UpdateCoroutine = this.StartCoroutine(this.EndOfFrameCoroutine());
        this.UpdateComponentsOnCurrentState(true);
    }

    void OnDisable() {
        InputListener.OnNPress -= this.ToggleNoClip;
        InputListener.OnXPress -= this.ToggleRealisticPossession;
        InputListener.OnZPress -= this.Unpossess;
        InputListener.OnLeftButtonPress -= this.UsePrimarySkill;
        InputListener.OnRightButtonHold -= this.OnRightMouseButtonHold;

        this.StopCoroutine(this.UpdateCoroutine);
        this.UpdateComponentsOnCurrentState(false);
    }

    void OnRightMouseButtonHold(bool isPressed) {
        if (isPressed) {
            this.UseSecondarySkill();
        }

        else {
            this.ReleaseSecondarySkill();
        }
    }

    void ToggleRealisticPossession() {
        Setting.RealisticPossessionEnabled = !Setting.RealisticPossessionEnabled;
        Chat.Print($"Realistic Possession: {Setting.RealisticPossessionEnabled}");

        if (this.EnemyToPossess?.agent.Unfake() is not NavMeshAgent navMeshAgent) {
            return;
        }

        navMeshAgent.updatePosition = Setting.RealisticPossessionEnabled;
        navMeshAgent.updateRotation = Setting.RealisticPossessionEnabled;
    }

    void ToggleNoClip() {
        this.NoClipEnabled = !this.NoClipEnabled;
        this.UpdateComponentsOnCurrentState(this.enabled);

        Chat.Print($"Possess NoClip: {this.NoClipEnabled}");
    }

    void UpdateComponentsOnCurrentState(bool thisGameObjectIsEnabled) {
        if (this.MousePan is not MousePan mousePan) return;
        if (this.RigidbodyKeyboard is not RigidbodyMovement rigidbodyKeyboard) return;
        if (this.Keyboard is not KeyboardMovement keyboard) return;

        mousePan.enabled = thisGameObjectIsEnabled;
        rigidbodyKeyboard.enabled = !this.NoClipEnabled;
        keyboard.enabled = this.NoClipEnabled;
    }

    IEnumerator EndOfFrameCoroutine() {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (true) {
            yield return waitForEndOfFrame;
            this.EndOfFrameUpdate();
        }
    }

    void HandleEnemyMovements(IController controller, EnemyAI enemy, bool isMoving, bool isSprinting) => controller.OnMovement(enemy, isMoving, isSprinting);

    void EnemyUpdate(IController controller, EnemyAI enemy) => controller.Update(enemy);

    // Updates position and rotation of possessed enemy at the end of frame
    void EndOfFrameUpdate() {
        if (this.RigidbodyKeyboard is not RigidbodyMovement rigidbodyKeyboard) return;
        if (this.EnemyToPossess is not EnemyAI enemy) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.CurrentCamera is not Camera camera || !camera.enabled) return;

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);

        if (this.FirstUpdate) {
            this.FirstUpdate = false;
            this.SetEnemyColliders(enemy, false);

            if (enemy.agent.Unfake() is NavMeshAgent agent) {
                agent.updatePosition = Setting.RealisticPossessionEnabled;
                agent.updateRotation = Setting.RealisticPossessionEnabled;
            }

            rigidbodyKeyboard.Init();
            this.transform.position = enemy.transform.position;
            this.UpdateComponentsOnCurrentState(true);
        }

        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) {
            this.UpdateEnemyPositionToHere(enemy);
        }

        else if (controller.IsAbleToMove(enemy)) {
            this.UpdateEnemyPositionToHere(enemy);
            this.HandleEnemyMovements(controller, enemy, rigidbodyKeyboard.IsMoving, rigidbodyKeyboard.IsSprinting);
            this.EnemyUpdate(controller, enemy);
        }

        camera.transform.position = this.transform.position + (3.0f * (Vector3.up - enemy.transform.forward));
        camera.transform.rotation = this.transform.rotation;
    }

    // Updates enemy's position to match the possessed object's position
    void UpdateEnemyPositionToHere(EnemyAI enemy) {
        enemy.updatePositionThreshold = 0;
        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;
        enemy.transform.eulerAngles = enemyEuler;
        enemy.transform.position = this.transform.position;
    }

    // Disables/enables colliders of the possessed enemy
    void SetEnemyColliders(EnemyAI enemy, bool enabled) =>
        enemy.GetComponentsInChildren<Collider>().ForEach(collider => collider.enabled = enabled);

    // Possesses the specified enemy
    internal void Possess(EnemyAI enemy) {
        this.Unpossess();

        this.EnemyToPossess = enemy;
        this.FirstUpdate = true;
    }

    // Releases possession of the current enemy
    internal void Unpossess() {
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
    }

    void UsePrimarySkill() {
        if (this.EnemyToPossess is not EnemyAI enemy) return;
        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

        controller.UsePrimarySkill(enemy);
    }

    void UseSecondarySkill() {
        if (this.EnemyToPossess is not EnemyAI enemy) return;
        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

        controller.UseSecondarySkill(enemy);
    }

    void ReleaseSecondarySkill() {
        if (this.EnemyToPossess is not EnemyAI enemy) return;
        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

        controller.ReleaseSecondarySkill(enemy);
    }
}
