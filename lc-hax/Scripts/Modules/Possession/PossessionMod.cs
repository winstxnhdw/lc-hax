using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameNetcodeStuff;
using Hax;

internal sealed class PossessionMod : MonoBehaviour {
    internal static PossessionMod? Instance { get; private set; }
    internal bool IsPossessed => this.Possession.IsPossessed;

    Possession Possession { get; } = new();
    Coroutine? UpdateCoroutine { get; set; } = null;
    CharacterMovement? RigidbodyKeyboard { get; set; } = null;
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
        this.RigidbodyKeyboard = this.gameObject.AddComponent<CharacterMovement>();
        this.Keyboard = this.gameObject.AddComponent<KeyboardMovement>();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        this.enabled = false;

        PossessionMod.Instance = this;
    }

    void OnEnable() {
        InputListener.OnNPress += this.ToggleNoClip;
        InputListener.OnZPress += this.Unpossess;
        InputListener.OnLeftButtonPress += this.UsePrimarySkill;
        InputListener.OnRightButtonHold += this.OnRightMouseButtonHold;

        this.UpdateCoroutine = this.StartCoroutine(this.EndOfFrameCoroutine());
        this.UpdateComponentsOnCurrentState(true);
    }

    void OnDisable() {
        InputListener.OnNPress -= this.ToggleNoClip;
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

    void ToggleNoClip() {
        this.NoClipEnabled = !this.NoClipEnabled;
        this.UpdateComponentsOnCurrentState(this.enabled);

        Helper.SendNotification(
            "Possess NoClip:",
            this.NoClipEnabled ? "Enabled" : "Disabled"
        );
    }

    void UpdateComponentsOnCurrentState(bool thisGameObjectIsEnabled) {
        if (this.MousePan is not MousePan mousePan) return;
        if (this.RigidbodyKeyboard is not CharacterMovement rigidbodyKeyboard) return;
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
        if (this.RigidbodyKeyboard is not CharacterMovement rigidbodyKeyboard) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.CurrentCamera is not Camera camera || !camera.enabled) return;

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);

        if (this.FirstUpdate) {
            this.FirstUpdate = false;
            this.SetEnemyColliders(enemy, false);

            if (enemy.agent is NavMeshAgent agent) {
                agent.updatePosition = false;
                agent.updateRotation = false;
            }

            rigidbodyKeyboard.Init();
            this.transform.position = enemy.transform.position;
            this.UpdateComponentsOnCurrentState(true);
        }

        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) {
            this.UpdateEnemyPositionToHere(enemy);
            this.UpdateCameraPosition(camera, enemy);
            return;
        }

        else if (controller.IsAbleToMove(enemy)) {
            this.UpdateEnemyPositionToHere(enemy);
            this.HandleEnemyMovements(controller, enemy, rigidbodyKeyboard.IsMoving, rigidbodyKeyboard.IsSprinting);
            this.EnemyUpdate(controller, enemy);
        }

        localPlayer.cursorTip.text = controller.GetPrimarySkillName(enemy);
        this.UpdateCameraPosition(camera, enemy);
    }

    void UpdateCameraPosition(Camera camera, EnemyAI enemy) {
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

        this.Possession.SetEnemy(enemy);
        this.FirstUpdate = true;
    }

    // Releases possession of the current enemy
    internal void Unpossess() {
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent.Unfake() is NavMeshAgent navMeshAgent) {
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = true;

            this.UpdateEnemyPositionToHere(enemy);
            _ = enemy.agent.Warp(enemy.transform.position);
        }

        this.SetEnemyColliders(enemy, true);
        this.Possession.Clear();
    }

    void UsePrimarySkill() {
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

        controller.UsePrimarySkill(enemy);
    }

    void UseSecondarySkill() {
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

        controller.UseSecondarySkill(enemy);
    }

    void ReleaseSecondarySkill() {
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

        controller.ReleaseSecondarySkill(enemy);
    }
}
