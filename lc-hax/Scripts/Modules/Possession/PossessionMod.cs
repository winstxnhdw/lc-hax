using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameNetcodeStuff;
using Hax;
using Unity.Netcode;
using System.Linq;

internal sealed class PossessionMod : MonoBehaviour {
    internal static PossessionMod? Instance { get; private set; }
    internal bool IsPossessed => this.Possession.IsPossessed;
    internal EnemyAI? PossessedEnemy => this.Possession.Enemy;

    Possession Possession { get; } = new();
    GameObject? CharacterMovementInstance { get; set; } = null;
    CharacterMovement? CharacterMovement { get; set; } = null;
    MousePan? MousePan { get; set; } = null;

    bool FirstUpdate { get; set; } = true;
    bool NoClipEnabled { get; set; } = false;

    bool IsAiControlled { get; set; } = false;

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
        { typeof(SpringManAI), new SpringManEnemyController() },
        { typeof(BlobAI), new BlobController() },
        { typeof(TestEnemy), new TestEnemyController() },
        { typeof(CrawlerAI), new CrawlerController()}
    };


    const float TeleportDoorCooldown = 2.5f;
    const float DoorInteractionCooldown = 0.7f;

    float doorCooldownRemaining = 0f;
    float teleportCooldownRemaining = 0f;
    EntranceTeleport? mainEntrance;


    void Awake() {
        this.InitCharacterMovement();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        this.enabled = false;


        PossessionMod.Instance = this;
    }

    void InitCharacterMovement(EnemyAI? enemy = null) {
        this.CharacterMovementInstance = new GameObject("Hax CharacterMovement");
        this.CharacterMovementInstance.transform.position = enemy == null ? default : enemy.transform.position;
        this.CharacterMovement = this.CharacterMovementInstance.AddComponent<CharacterMovement>();
        this.CharacterMovement.Init();
        if (enemy != null) {
            this.CharacterMovement.CalibrateCollision(enemy);
            this.CharacterMovement.CharacterSprintSpeed = this.SprintMultiplier(enemy);
        }

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
        InputListener.OnF9Press += this.ToggleAiControl;
        this.UpdateComponentsOnCurrentState(true);
    }

    void OnDisable() {
        InputListener.OnNPress -= this.ToggleNoClip;
        InputListener.OnZPress -= this.Unpossess;
        InputListener.OnLeftButtonPress -= this.UsePrimarySkill;
        InputListener.OnRightButtonHold -= this.OnRightMouseButtonHold;
        InputListener.OnDelPress -= this.KillEnemyAndUnposses;
        InputListener.OnF9Press -= this.ToggleAiControl;
        this.UpdateComponentsOnCurrentState(false);
    }

    void OnRightMouseButtonHold(bool isPressed) {
        if (isPressed)
            this.UseSecondarySkill();

        else
            this.ReleaseSecondarySkill();
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
        if (this.MousePan == null) return;
        if (this.CharacterMovement == null) return;
        this.MousePan.enabled = thisGameObjectIsEnabled;
        this.CharacterMovement.gameObject.SetActive(thisGameObjectIsEnabled);
        this.CharacterMovement.SetNoClipMode(this.NoClipEnabled);
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

    public void Update() {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent is not NavMeshAgent nav) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.CurrentCamera is not Camera { enabled: true } camera) return;

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);

        if (this.FirstUpdate) {
            this.FirstUpdate = false;
            this.InitCharacterMovement(enemy);
            this.UpdateComponentsOnCurrentState(true);
            this.SetAiControl(false);
            if (this.mainEntrance == null) this.mainEntrance = FindObjectsOfType<EntranceTeleport>().FirstOrDefault(entrance => entrance.entranceId == 0);
        }

        if (this.mainEntrance != null)
            enemy.SetOutsideStatus(enemy.transform.position.y > this.mainEntrance.transform.position.y + 5f);

        if (!this.IsAiControlled) {
            if (this.SyncAnimationSpeedEnabled(enemy)) characterMovement.CharacterSpeed = nav.speed;

            if (!this.EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) {
                this.UpdateEnemyPosition(enemy);
                this.UpdateEnemyRotation();
            }
            else if (controller.IsAbleToMove(enemy)) {
                this.UpdateEnemyPosition(enemy);
                this.HandleEnemyMovements(controller, enemy, characterMovement.IsMoving, characterMovement.IsSprinting);
                this.EnemyUpdate(controller, enemy);
                if (controller.IsAbleToRotate(enemy)) this.UpdateEnemyRotation();
                localPlayer.cursorTip.text = controller.GetPrimarySkillName(enemy);
            }
        }

        this.UpdateCameraPosition(camera, enemy);
        this.UpdateCameraRotation(camera, enemy);
        this.InteractWithAmbient();
    }

    void UpdateCameraPosition(Camera camera, EnemyAI enemy) =>
        camera.transform.position = enemy.transform.position + (3.0f * (Vector3.up - enemy.transform.forward));


    void UpdateCameraRotation(Camera camera, EnemyAI enemy) {
        if (enemy == null) return;
        camera.transform.rotation = !this.IsAiControlled ? this.transform.rotation : Quaternion.LookRotation(enemy.transform.forward);
    }



    void UpdateEnemyRotation() {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
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

    // Possesses the specified enemy
    internal void Possess(EnemyAI enemy) {
        this.Unpossess();
        if (enemy.isEnemyDead) return;

        this.mainEntrance = null;
        this.FirstUpdate = true;
        this.Possession.SetEnemy(enemy);
        if (this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value))
            this.HandleEnemyOnPossess(value, enemy);
        this.IsAiControlled = false;
        this.ResetInteractionCooldowns();
    }

    void KillEnemyAndUnposses() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        enemy.Kill(localPlayer.actualClientId);
        if (enemy.TryGetComponent(out NetworkObject networkObject)) networkObject.Despawn(true);
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

        if (this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value))
            this.HandleEnemyOnUnpossess(value, enemy);

        this.IsAiControlled = false;
        this.ResetInteractionCooldowns();
        this.Possession.Clear();
    }

    internal void ToggleAiControl() {
        if (this.Possession.Enemy is null || this.Possession.Enemy.agent.Unfake() is null ||
            this.CharacterMovement is null || this.MousePan is null) return;
        this.IsAiControlled = !this.IsAiControlled;
        this.SetAiControl(this.IsAiControlled, true);
    }

    internal void SetAiControl(bool EnableAI, bool DisplayNotification = false) {
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent.Unfake() is not NavMeshAgent navMeshAgent) return;
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;

        if (EnableAI) {
            _ = enemy.agent.Warp(enemy.transform.position);
            enemy.SyncPositionToClients();
        }

        if (this.NoClipEnabled) {
            this.NoClipEnabled = false;
            characterMovement.SetNoClipMode(false);
        }

        navMeshAgent.updatePosition = EnableAI;
        navMeshAgent.updateRotation = EnableAI;
        navMeshAgent.isStopped = !EnableAI;
        characterMovement.SetPosition(enemy.transform.position);
        characterMovement.enabled = !EnableAI;
        if (DisplayNotification) {
            Helper.SendNotification(
                title: "AI Control:",
                body: this.IsAiControlled ? "Enabled" : "Disabled"
            );
        }

    }

    public void ResetInteractionCooldowns() {
        this.teleportCooldownRemaining = -DoorInteractionCooldown;
        this.doorCooldownRemaining = -TeleportDoorCooldown;
    }


    bool CanUseEntranceDoors(EnemyAI enemy) {
        return enemy is EnemyAI enemyAI
&& this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value) && value.CanUseEntranceDoors(enemyAI);
    }

    bool SyncAnimationSpeedEnabled(EnemyAI enemy) {
        return enemy is EnemyAI enemyAI
&& this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value) && value.SyncAnimationSpeedEnabled(enemyAI);
    }


    float InteractRange(EnemyAI enemy) {
        return enemy is not EnemyAI enemyAI
            ? 0
            : this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value)
            ? value.InteractRange(enemyAI).GetValueOrDefault(2.5f)
            : 2.5f;
    }

    float SprintMultiplier(EnemyAI enemy) {
        return enemy is not EnemyAI enemyAI
            ? 0
            : this.EnemyControllers.TryGetValue(enemy.GetType(), out IController value)
            ? value.SprintMultiplier(enemyAI).GetValueOrDefault(2.8f)
            : 2.8f;
    }


    void InteractWithAmbient() {
        if (this.doorCooldownRemaining > 0) this.doorCooldownRemaining -= Time.deltaTime;
        if (this.teleportCooldownRemaining > 0) this.teleportCooldownRemaining -= Time.deltaTime;
        if (this.Possession.Enemy is not EnemyAI enemy) return;


        if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out RaycastHit hit,
                this.InteractRange(enemy))) {
            if (hit.collider.gameObject.TryGetComponent(out DoorLock doorLock) && this.doorCooldownRemaining <= 0) {
                this.OpenDoorAsEnemy(doorLock);
                this.doorCooldownRemaining = DoorInteractionCooldown;
                return;
            }

            if (this.CanUseEntranceDoors(enemy))
                if (hit.collider.gameObject.TryGetComponent(out EntranceTeleport entrance) &&
                    this.teleportCooldownRemaining <= 0) {
                    this.InteractWithTeleport(enemy, entrance);
                    this.teleportCooldownRemaining = TeleportDoorCooldown;
                    return;
                }
        }
    }

    void OpenDoorAsEnemy(DoorLock door) {
        if (!door.Reflect().GetInternalField<bool>("isDoorOpened")) {
            door.OpenDoorAsEnemyServerRpc();
            if (door.gameObject.TryGetComponent(out AnimatedObjectTrigger trigger))
                trigger.TriggerAnimationNonPlayer(false, true, false);
        }
    }


    Transform? GetExitPointFromDoor(EntranceTeleport entrance) {
        if (entrance == null) return null;
        EntranceTeleport[] allTeleports = FindObjectsOfType<EntranceTeleport>();
        for (int index = 0; index < allTeleports.Length; index++) {
            EntranceTeleport teleport = allTeleports[index];
            if (teleport.entranceId == entrance.entranceId &&
                teleport.isEntranceToBuilding != entrance.isEntranceToBuilding)
                return teleport.entrancePoint;
        }

        return null;
    }


    public void InteractWithTeleport(EnemyAI Enemy, EntranceTeleport teleport) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        Transform? exitPoint = this.GetExitPointFromDoor(teleport);
        if (exitPoint == null) return;
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
