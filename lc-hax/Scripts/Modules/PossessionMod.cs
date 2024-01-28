using UnityEngine;
using System.Collections;
using GameNetcodeStuff;
using UnityEngine.AI;
using Hax;

public sealed class PossessionMod : MonoBehaviour {
    // Vector3 to store the enemy's position before possession
    Vector3 savedEnemyPosition;

    // Nullable property representing the enemy to possess
    EnemyAI? EnemyToPossess { get; set; } = null;

    // Flag to determine if it's the first update after possession
    bool FirstUpdate { get; set; } = true;

    // Nullable properties for various movement components
    RigidbodyMovement? RigidbodyKeyboard { get; set; } = null;
    KeyboardMovement? Keyboard { get; set; } = null;
    MousePan? MousePan { get; set; } = null;

    // Singleton instance of the PossessionMod
    public static PossessionMod? Instance { get; private set; }

    // Readonly property indicating if possessing an enemy
    public bool IsPossessed => this.EnemyToPossess != null;

    // Flag for no clipping mode
    bool noClip = false;

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
        this.noClip = !this.noClip;
        Chat.Print($"Possess NoClip: {this.noClip}");

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
        rigidbodyKeyboard.enabled = !this.noClip;
        keyboard.enabled = this.noClip;
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

    // Updates position and rotation of possessed enemy at the end of frame
    private void EndOfFrameUpdate() {
        if (this.RigidbodyKeyboard is not RigidbodyMovement rigidbodyKeyboard) return;
        if (this.EnemyToPossess is not EnemyAI enemy) return;
        if (Helper.CurrentCamera is not Camera camera || !camera.enabled) return;

        if (this.FirstUpdate) {
            // Save enemy's position before possession
            this.savedEnemyPosition = enemy.transform.position;

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
        camera.transform.position = this.transform.position + (Vector3.up * 3f) - (enemy.transform.forward * 3f);
        camera.transform.rotation = this.transform.rotation;

        this.FirstUpdate = false;
    }

    // Updates enemy's position to match the possessed object's position
    void UpdateEnemyPositionToHere(EnemyAI enemy) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

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

        // Delayed teleportation to saved position
        StartCoroutine(TeleportToSavedPositionCoroutine());
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
    }

    // Coroutine for delayed teleportation
    IEnumerator TeleportToSavedPositionCoroutine() {
        yield return new WaitForSeconds(0.08f); // Adjust the delay as needed

        // Teleport to saved position after delay
        this.transform.position = this.savedEnemyPosition;
    }
}
