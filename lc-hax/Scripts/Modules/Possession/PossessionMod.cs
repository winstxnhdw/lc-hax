using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using GameNetcodeStuff;
using Hax;

internal sealed class PossessionMod : MonoBehaviour {
    const float TeleportDoorCooldown = 2.5f;
    const float DoorInteractionCooldown = 0.7f;

    internal static PossessionMod? Instance { get; private set; }
    internal bool IsPossessed => this.Possession.IsPossessed;
    internal EnemyAI? PossessedEnemy => this.Possession.Enemy;

    Possession Possession { get; } = new();
    GameObject? CharacterMovementInstance { get; set; } = null;
    CharacterMovement? CharacterMovement { get; set; } = null;
    MousePan? MousePan { get; set; } = null;

    bool FirstUpdate { get; set; } = true;
    bool NoClipEnabled { get; set; } = false;
    bool IsAIControlled { get; set; } = false;

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
        { typeof(SpringManAI), new CoilHeadController() },
        { typeof(BlobAI), new HygrodereController() },
        { typeof(TestEnemy), new TestEnemyController() },
        { typeof(LassoManAI), new LassoManController() },
        { typeof(CrawlerAI), new CrawlerController() },
        { typeof(SandSpiderAI), new SandSpiderController() },
        { typeof(RedLocustBees), new CircuitBeesController() }
    };

    float DoorCooldownRemaining { get; set; } = 0.0f;
    float TeleportCooldownRemaining { get; set; } = 0.0f;
    EntranceTeleport? MainEntrance { get; set; }

    void Awake() {
        this.InitCharacterMovement();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        this.enabled = false;

        PossessionMod.Instance = this;
    }

    void InitCharacterMovement(EnemyAI? enemy = null) {
        this.CharacterMovementInstance = new GameObject("Hax CharacterMovement");
        this.CharacterMovementInstance.transform.position = enemy is null ? default : enemy.transform.position;
        this.CharacterMovement = this.CharacterMovementInstance.AddComponent<CharacterMovement>();
        this.CharacterMovement.Init();

        if (enemy is not null) {
            this.CharacterMovement.CalibrateCollision(enemy);
            this.CharacterMovement.CharacterSprintSpeed = this.SprintMultiplier(enemy);
        }
    }

    void OnEnable() {
        InputListener.OnNPress += this.ToggleNoClip;
        InputListener.OnZPress += this.Unpossess;
        InputListener.OnLeftButtonPress += this.UsePrimarySkill;
        InputListener.OnRightButtonPress += this.UseSecondarySkill;
        InputListener.OnRightButtonRelease += this.ReleaseSecondarySkill;
        InputListener.OnRightButtonHold += this.OnRightMouseButtonHold;
        InputListener.OnDelPress += this.KillEnemyAndUnposses;
        InputListener.OnF9Press += this.ToggleAIControl;
        this.UpdateComponentsOnCurrentState(true);
    }

    void OnDisable() {
        InputListener.OnNPress -= this.ToggleNoClip;
        InputListener.OnZPress -= this.Unpossess;
        InputListener.OnLeftButtonPress -= this.UsePrimarySkill;
        InputListener.OnRightButtonPress -= this.UseSecondarySkill;
        InputListener.OnRightButtonRelease -= this.ReleaseSecondarySkill;
        InputListener.OnRightButtonHold -= this.OnRightMouseButtonHold;
        InputListener.OnDelPress -= this.KillEnemyAndUnposses;
        InputListener.OnF9Press -= this.ToggleAIControl;
        this.UpdateComponentsOnCurrentState(false);
    }

    void OnRightMouseButtonHold(bool isPressed) {
        if (isPressed) {
            this.OnSecondarySkillHold();
        }
    }

    void SendPossessionNotifcation(string message) {
        Helper.SendNotification(
            title: "Possession",
            body: message
        );
    }

    void ToggleNoClip() {
        this.NoClipEnabled = !this.NoClipEnabled;
        this.UpdateComponentsOnCurrentState(this.enabled);
        this.SendPossessionNotifcation($"NoClip: {(this.NoClipEnabled ? "Enabled" : "Disabled")}");
    }

    void UpdateComponentsOnCurrentState(bool thisGameObjectIsEnabled) {
        if (this.MousePan is null) return;

        this.MousePan.enabled = thisGameObjectIsEnabled;
        this.CharacterMovement?.gameObject.SetActive(thisGameObjectIsEnabled);
        this.CharacterMovement?.SetNoClipMode(this.NoClipEnabled);
    }

    void Update() {
        if (Helper.CurrentCamera is not Camera { enabled: true } camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent is not NavMeshAgent agent) return;

        this.DoorCooldownRemaining = Mathf.Clamp(
            this.DoorCooldownRemaining - Time.deltaTime,
            0.0f,
            PossessionMod.DoorInteractionCooldown
        );

        this.TeleportCooldownRemaining = Mathf.Clamp(
            this.TeleportCooldownRemaining - Time.deltaTime,
            0.0f,
            PossessionMod.TeleportDoorCooldown
        );

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        this.UpdateCameraPosition(camera, enemy);
        this.UpdateCameraRotation(camera, enemy);

        if (this.FirstUpdate) {
            this.FirstUpdate = false;
            this.InitCharacterMovement(enemy);
            this.UpdateComponentsOnCurrentState(true);
            this.SetAIControl(false);
            this.MainEntrance = RoundManager.FindMainEntranceScript(false);
        }


        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) {
            if (!this.IsAIControlled) {
                this.UpdateEnemyPosition(enemy);
                this.UpdateEnemyRotation();
            }

            if (this.MainEntrance != null) {
                _ = enemy.SetOutsideStatus(enemy.transform.position.y > this.MainEntrance.transform.position.y + 5.0f);
            }

            if (enemy.isEnemyDead) {
                this.Unpossess();
            }

            return;
        }

        if (this.MainEntrance != null) {
            if (enemy.SetOutsideStatus(enemy.transform.position.y > this.MainEntrance.transform.position.y + 5.0f)) {
                controller.OnOutsideStatusChange(enemy);
                enemy.FinishedCurrentSearchRoutine();
            }
        }

        if (enemy.isEnemyDead) {
            controller.OnDeath(enemy);
            this.Unpossess();
        }

        controller.Update(enemy, this.IsAIControlled);
        this.InteractWithAmbient(enemy, controller);
        localPlayer.cursorTip.text = controller.GetPrimarySkillName(enemy);

        if (this.IsAIControlled) {
            return;
        }

        if (!controller.IsAbleToMove(enemy)) {
            return;
        }

        if (controller.SyncAnimationSpeedEnabled(enemy)) {
            characterMovement.CharacterSpeed = agent.speed;
        }

        if (controller.IsAbleToRotate(enemy)) {
            this.UpdateEnemyRotation();
        }

        this.UpdateEnemyPosition(enemy);
        controller.OnMovement(enemy, this.CharacterMovement.IsMoving, this.CharacterMovement.IsSprinting);
    }

    void UpdateCameraPosition(Camera camera, EnemyAI enemy) =>
        camera.transform.position = enemy.transform.position + (3.0f * (Vector3.up - enemy.transform.forward));

    void UpdateCameraRotation(Camera camera, EnemyAI enemy) =>
        camera.transform.rotation = !this.IsAIControlled
            ? this.transform.rotation
            : Quaternion.LookRotation(enemy.transform.forward);

    void UpdateEnemyRotation() {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        characterMovement.transform.rotation = this.transform.rotation;
    }

    // Updates enemy's position to match the possessed object's position
    void UpdateEnemyPosition(EnemyAI enemy) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;

        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;

        enemy.transform.eulerAngles = enemyEuler;
        enemy.transform.position = characterMovement.transform.position;
    }

    // Possesses the specified enemy
    internal void Possess(EnemyAI enemy) {
        if (enemy.isEnemyDead) return;

        this.Unpossess();
        this.FirstUpdate = true;
        this.Possession.SetEnemy(enemy);
        this.IsAIControlled = false;
        this.TeleportCooldownRemaining = 0.0f;
        this.DoorCooldownRemaining = 0.0f;

        if (this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) {
            controller.OnPossess(enemy);
        }
    }

    void KillEnemyAndUnposses() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;

        enemy.Kill(localPlayer.actualClientId);

        if (localPlayer.IsHost) {
            if (enemy.TryGetComponent(out NetworkObject networkObject)) {
                networkObject.Despawn(true);
            }
        }

        this.Unpossess();
    }

    // Releases possession of the current enemy
    internal void Unpossess() {
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent is NavMeshAgent agent) {
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;
            this.UpdateEnemyPosition(enemy);
            _ = enemy.agent.Warp(enemy.transform.position);
            enemy.SyncPositionToClients();
        }

        if (this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) {
            controller.OnUnpossess(enemy);
        }

        this.IsAIControlled = false;
        this.MainEntrance = null;
        this.Possession.Clear();

        if (this.CharacterMovement is not null) {
            Destroy(this.CharacterMovementInstance);
            this.CharacterMovementInstance = null;
        }
    }

    void ToggleAIControl() {
        if (this.Possession.Enemy?.agent is null) return;
        if (this.CharacterMovement is null) return;
        if (this.MousePan is null) return;

        this.IsAIControlled = !this.IsAIControlled;
        this.SetAIControl(this.IsAIControlled);
        this.SendPossessionNotifcation($"AI Control: {(this.IsAIControlled ? "Enabled" : "Disabled")}");
    }

    void SetAIControl(bool enableAI) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent is not NavMeshAgent agent) return;
        if (enableAI) {
            _ = enemy.agent.Warp(enemy.transform.position);
            enemy.SyncPositionToClients();
        }

        if (this.NoClipEnabled) {
            this.NoClipEnabled = false;
            characterMovement.SetNoClipMode(false);
        }

        agent.updatePosition = enableAI;
        agent.updateRotation = enableAI;
        agent.isStopped = !enableAI;
        characterMovement.SetPosition(enemy.transform.position);
        characterMovement.enabled = !enableAI;
    }

    void HandleEntranceDoors(EnemyAI enemy, RaycastHit hit, IController controller) {
        if (this.TeleportCooldownRemaining > 0.0f) return;
        if (!hit.collider.gameObject.TryGetComponent(out EntranceTeleport entrance)) return;

        this.InteractWithTeleport(enemy, entrance, controller);
        this.TeleportCooldownRemaining = PossessionMod.TeleportDoorCooldown;
    }

    void HandleEnemyPlayerInteraction(EnemyAI enemy, RaycastHit hit, IController controller) {
        if (!hit.collider.gameObject.TryGetComponent(out PlayerControllerB player)) return;
        controller.OnCollideWithPlayer(enemy, player);
    }


    float InteractRange(EnemyAI enemy) =>
        this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value)
            ? value.InteractRange(enemy)
            : IController.DefaultInteractRange;

    float SprintMultiplier(EnemyAI enemy) =>
        this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value)
            ? value.SprintMultiplier(enemy)
            : IController.DefaultSprintMultiplier;

    void InteractWithAmbient(EnemyAI enemy, IController controller) {
        if (!Physics.Raycast(enemy.transform.position, enemy.transform.forward, out RaycastHit hit, this.InteractRange(enemy))) return;
        if (hit.collider.gameObject.TryGetComponent(out DoorLock doorLock) && this.DoorCooldownRemaining <= 0.0f) {
            this.OpenDoorAsEnemy(doorLock);
            this.DoorCooldownRemaining = PossessionMod.DoorInteractionCooldown;
            return;
        }

        if (controller.CanUseEntranceDoors(enemy)) {
            this.HandleEntranceDoors(enemy, hit, controller);
            return;
        }

        if (controller.CanUseEntranceDoors(enemy)) {
            this.HandleEntranceDoors(enemy, hit, controller);
            return;
        }

        this.HandleEnemyPlayerInteraction(enemy, hit, controller);
    }

    void OpenDoorAsEnemy(DoorLock door) {
        if (door.Reflect().GetInternalField<bool>("isDoorOpened")) return;
        if (door.gameObject.TryGetComponent(out AnimatedObjectTrigger trigger)) {
            trigger.TriggerAnimationNonPlayer(false, true, false);
        }

        door.OpenDoorAsEnemyServerRpc();
    }

    Transform? GetExitPointFromDoor(EntranceTeleport entrance) =>
        Helper.FindObjects<EntranceTeleport>().First(teleport =>
            teleport.entranceId == entrance.entranceId && teleport.isEntranceToBuilding != entrance.isEntranceToBuilding
        )?.entrancePoint;

    void InteractWithTeleport(EnemyAI enemy, EntranceTeleport teleport, IController controller) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (this.GetExitPointFromDoor(teleport) is not Transform exitPoint) return;
        characterMovement.SetPosition(exitPoint.position);
        _ = enemy.SetOutsideStatus(!teleport.isEntranceToBuilding);
        controller.OnOutsideStatusChange(enemy);
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

    void OnSecondarySkillHold() {
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

        controller.OnSecondarySkillHold(enemy);
    }

    void ReleaseSecondarySkill() {
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

        controller.ReleaseSecondarySkill(enemy);
    }
}
