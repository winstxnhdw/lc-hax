using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameNetcodeStuff;
using Hax;

internal sealed class PossessionMod : MonoBehaviour {
    internal static PossessionMod? Instance { get; private set; }
    internal bool IsPossessed => this.Possession.IsPossessed;

    Possession Possession { get; } = new();
    GameObject? CharacterMovementInstance { get; set; } = null;
    CharacterMovement? CharacterMovement { get; set; } = null;
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
        { typeof(MaskedPlayerEnemy), new MaskedPlayerController() },
        { typeof(SpringManAI), new SpringManEnemyController() }

    };
    void Awake() {
        this.InitCharacterMovement();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        this.enabled = false;

        PossessionMod.Instance = this;
    }

    void InitCharacterMovement(EnemyAI? enemy = null) {
        this.CharacterMovementInstance = new GameObject("Hax_CharacterMovement");
        this.CharacterMovementInstance.transform.position = enemy == null ? default : enemy.transform.position;
        this.CharacterMovement = this.CharacterMovementInstance.AddComponent<CharacterMovement>();
        this.CharacterMovement.Init();
        if (enemy != null) this.CharacterMovement.CalibrateCollision(enemy);
        DontDestroyOnLoad(this.CharacterMovementInstance);
    }

    void UnInitCharacterMovement() {
        if (this.CharacterMovementInstance is not GameObject characterMovementInstance) return;

        Destroy(characterMovementInstance);
    }

    void OnEnable() {
        InputListener.OnNPress += this.ToggleNoClip;
        InputListener.OnZPress += this.Unpossess;
        InputListener.OnLeftButtonPress += this.UsePrimarySkill;
        InputListener.OnRightButtonHold += this.OnRightMouseButtonHold;
        InputListener.OnDelPress += this.KillEnemyAndUnposses;

        this.UpdateComponentsOnCurrentState(true);
    }

    void OnDisable() {
        InputListener.OnNPress -= this.ToggleNoClip;
        InputListener.OnZPress -= this.Unpossess;
        InputListener.OnLeftButtonPress -= this.UsePrimarySkill;
        InputListener.OnRightButtonHold -= this.OnRightMouseButtonHold;
        InputListener.OnDelPress -= this.KillEnemyAndUnposses;

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
            title: "Possess NoClip:",
            body: this.NoClipEnabled ? "Enabled" : "Disabled"
        );
    }

    void UpdateComponentsOnCurrentState(bool thisGameObjectIsEnabled) {
        if (this.MousePan is not MousePan mousePan) return;
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;

        mousePan.enabled = thisGameObjectIsEnabled;
        characterMovement.gameObject.SetActive(thisGameObjectIsEnabled);
        characterMovement.SetNoClipMode(this.NoClipEnabled);
    }


    void HandleEnemyMovements(IController controller, EnemyAI enemy, bool isMoving, bool isSprinting) =>
        controller.OnMovement(enemy, isMoving, isSprinting);

    void HandleEnemyOnPossess(IController controller, EnemyAI enemy) =>
        controller.OnPossess(enemy);

    void HandleEnemyOnUnpossess(IController controller, EnemyAI enemy) =>
        controller.OnUnpossess(enemy);

    void HandleEnemyOnDeath(IController controller, EnemyAI enemy) =>
        controller.OnDeath(enemy);

    void EnemyUpdate(IController controller, EnemyAI enemy) => controller.Update(enemy);


    // Updates position and rotation of possessed enemy at the end of frame
    public void Update() {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.CurrentCamera is not Camera camera || !camera.enabled) return;

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);

        if (this.FirstUpdate) {
            this.FirstUpdate = false;
            //this.SetEnemyColliders(enemy, false);

            if (enemy.agent is NavMeshAgent agent) {
                agent.updatePosition = false;
                agent.updateRotation = false;
                agent.isStopped = true;
            }

            this.InitCharacterMovement(enemy);
            this.UpdateComponentsOnCurrentState(true);
        }

        if (enemy.isEnemyDead) {
            if (this.EnemyControllers.TryGetValue(enemy.GetType(), out IController Death)) {
                this.HandleEnemyOnDeath(Death, enemy);
            }

            this.Unpossess();
            return;
        }

        // this updates the character's movement speed to match the enemy speeds (sprinting, walking, etc)
        if (enemy.agent is NavMeshAgent nav) {
            characterMovement.CharacterSpeed = nav.speed;
        }

        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) {
            this.UpdateEnemyPosition(enemy);
            this.UpdateCameraPosition(camera, enemy);
            this.UpdateCameraRotation(camera, enemy);
            return;
        }

        else if (controller.IsAbleToMove(enemy)) {
            this.UpdateEnemyPosition(enemy);
            this.HandleEnemyMovements(controller, enemy, characterMovement.IsMoving, characterMovement.IsSprinting);
            this.EnemyUpdate(controller, enemy);
            this.UpdateCameraPosition(camera, enemy);
            if (controller.IsAbleToRotate(enemy)) {
                this.UpdateCameraRotation(camera, enemy);
            }
        }


        localPlayer.cursorTip.text = controller.GetPrimarySkillName(enemy);
        this.InteractWithAmbient();
    }

    void UpdateCameraPosition(Camera camera, EnemyAI enemy) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        camera.transform.position =
            characterMovement.transform.position + (3.0f * (Vector3.up - enemy.transform.forward));
    }

    void UpdateCameraRotation(Camera camera, EnemyAI enemy) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        camera.transform.rotation = this.transform.rotation;
        characterMovement.transform.rotation = this.transform.rotation;
    }

    // Updates enemy's position to match the possessed object's position
    void UpdateEnemyPosition(EnemyAI enemy) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;

        enemy.updatePositionThreshold = 0;
        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;
        enemy.transform.eulerAngles = enemyEuler;
        enemy.transform.position = characterMovement.transform.position;
    }

    // Disables/enables colliders of the possessed enemy
    void SetEnemyColliders(EnemyAI enemy, bool enabled) =>
        enemy.GetComponentsInChildren<Collider>().ForEach(collider => collider.enabled = enabled);

    // Possesses the specified enemy
    internal void Possess(EnemyAI enemy) {
        this.Unpossess();
        if (enemy.isEnemyDead) return;

        this.FirstUpdate = true;
        this.Possession.SetEnemy(enemy);
        if (this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value)) {
            this.HandleEnemyOnPossess(value, enemy);
        }
        this.ResetInteractionCooldowns();

    }

    void KillEnemyAndUnposses() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        this.Possession?.Enemy?.Kill(localPlayer.actualClientId);
        this.Unpossess();
    }

    // Releases possession of the current enemy
    internal void Unpossess() {
        this.UnInitCharacterMovement();
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent.Unfake() is NavMeshAgent navMeshAgent) {
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = true;
            navMeshAgent.isStopped = false;
            this.UpdateEnemyPosition(enemy);
            _ = enemy.agent.Warp(enemy.transform.position);
        }

        if (this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value)) {
            this.HandleEnemyOnUnpossess(value, enemy);
        }

        //this.SetEnemyColliders(enemy, true);
        this.ResetInteractionCooldowns();
        this.Possession.Clear();
    }

    public void ResetInteractionCooldowns() {
        this.teleportCooldownRemaining = -DoorInteractionCooldown;
        this.doorCooldownRemaining = -TeleportDoorCooldown;
    }

    const float TeleportDoorCooldown = 2.5f;
    const float DoorInteractionCooldown = 0.7f;

    float doorCooldownRemaining = 0f;
    float teleportCooldownRemaining = 0f;


    bool CanUseEntranceDoors(EnemyAI enemy) {
        return enemy is not EnemyAI enemyAI
            ? false
            : this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value) && value.CanUseEntranceDoors(enemyAI);
    }


    float InteractRange(EnemyAI enemy) {
        return enemy is not EnemyAI enemyAI
            ? 0
            : this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value)
            ? value.InteractRange(enemyAI).GetValueOrDefault(1.0f)
            : 1.0f;
    }



    void InteractWithAmbient() {
        if (this.doorCooldownRemaining > 0) this.doorCooldownRemaining -= Time.deltaTime;
        if (this.teleportCooldownRemaining > 0) this.teleportCooldownRemaining -= Time.deltaTime;
        if (this.Possession.Enemy is not EnemyAI enemy) return;


        if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out RaycastHit hit, this.InteractRange(enemy))) {
            if (hit.collider.gameObject.TryGetComponent(out DoorLock doorLock) && this.doorCooldownRemaining <= 0) {
                this.OpenDoorAsEnemy(doorLock);
                this.doorCooldownRemaining = DoorInteractionCooldown;
                return;
            }

            if (this.CanUseEntranceDoors(enemy)) {
                if (hit.collider.gameObject.TryGetComponent(out EntranceTeleport entrance) &&
                    this.teleportCooldownRemaining <= 0) {
                    this.InteractWithTeleport(enemy, entrance);
                    this.teleportCooldownRemaining = TeleportDoorCooldown;
                    return;
                }
            }
        }
    }

    void OpenDoorAsEnemy(DoorLock door) {
        if (!door.Reflect().GetInternalField<bool>("isDoorOpened")) {
            door.OpenDoorAsEnemyServerRpc();
            if (door.gameObject.TryGetComponent(out AnimatedObjectTrigger trigger)) {
                trigger.TriggerAnimationNonPlayer(false, true, false);
            }
        }
    }


    Transform? GetExitPointFromDoor(EntranceTeleport entrance) {
        if (entrance == null) {
            return null;
        }
        EntranceTeleport[] allTeleports = FindObjectsOfType<EntranceTeleport>();
        for (int index = 0; index < allTeleports.Length; index++) {
            EntranceTeleport teleport = allTeleports[index];
            if (teleport.entranceId == entrance.entranceId &&
                teleport.isEntranceToBuilding != entrance.isEntranceToBuilding) {
                return teleport.entrancePoint;
            }
        }

        return null;
    }



    public void InteractWithTeleport(EnemyAI Enemy, EntranceTeleport teleport) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        Transform? exitPoint = this.GetExitPointFromDoor(teleport);
        if (exitPoint == null) return;
        Enemy.isOutside = !teleport.isEntranceToBuilding;
        Enemy.allAINodes = Enemy.isOutside ? GameObject.FindGameObjectsWithTag("OutsideAINode") : GameObject.FindGameObjectsWithTag("AINode");
        characterMovement.SetPosition(exitPoint.position);
        Enemy.EnableEnemyMesh(true, false);
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
