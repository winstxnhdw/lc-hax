using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using GameNetcodeStuff;
using Hax;
using UnityEngine.Windows.WebCam;
using static UnityEngine.EventSystems.EventTrigger;
using Steamworks;

internal sealed class PossessionMod : MonoBehaviour {
    bool IsLeftAltHeld { get; set; } = false;

    internal static PossessionMod? Instance { get; private set; }
    internal bool IsPossessed => this.Possession.IsPossessed;
    internal EnemyAI? PossessedEnemy => this.Possession.Enemy;
    internal IController? Controller { get; private set; } = null;

    Possession Possession { get; } = new();
    GameObject? CharacterMovementInstance { get; set; } = null;
    CharacterMovement? CharacterMovement { get; set; } = null;
    EntranceTeleport? MainEntrance => RoundManager.FindMainEntranceScript(false);

    MousePan? MousePan { get; set; } = null;

    bool FirstUpdate { get; set; } = true;
    bool NoClipEnabled { get; set; } = false;
    bool IsAIControlled { get; set; } = false;


    // determines the controller to use for each enemy types 
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
        { typeof(SandSpiderAI), new BunkerSpiderController() },
        { typeof(RedLocustBees), new CircuitBeesController() },
        { typeof(DoublewingAI), new DoublewingBirdController() },
        { typeof(DocileLocustBeesAI), new DocileLocustBeesController() },
        { typeof(ButlerEnemyAI), new ButlerEnemyController() },
        { typeof(ButlerBeesEnemyAI), new ButlerBeesController() },
        { typeof(RadMechAI), new RadMechController() },
        { typeof(FlowerSnakeEnemy), new FlowerSnakeController() },
    };

    internal IController? GetEnemyController(EnemyAI enemy) =>
        enemy == null ? null : this.EnemyControllers.GetValueOrDefault(enemy.GetType());

    void Awake() {
        this.MousePan = this.gameObject.GetOrAddComponent<MousePan>();
        this.enabled = false;

        PossessionMod.Instance = this;
    }

    void InitCharacterMovement(EnemyAI? enemy = null) {
        this.CharacterMovementInstance = Finder.Find("Hax CharacterMovement");
        if (this.CharacterMovementInstance == null) {
            this.CharacterMovementInstance = new("Hax CharacterMovement");
            this.CharacterMovementInstance.transform.position = default;
        }
        if (enemy != null) {
            this.CharacterMovementInstance.transform.position = enemy.transform.position;
            this.CharacterMovement = this.CharacterMovementInstance.GetOrAddComponent<CharacterMovement>();
            if (this.CharacterMovement is not null) {
                this.CharacterMovement.SetPosition(enemy.transform.position);
                this.CharacterMovement.CharacterSprintSpeed = this.SprintMultiplier(enemy);
                this.CharacterMovement.CanMove = true;
            }
        }
    }



    void OnEnable() {
        InputListener.OnNPress += this.ToggleNoClip;
        InputListener.OnZPress += this.Unpossess;
        InputListener.OnLeftButtonPress += this.UsePrimarySkill;
        InputListener.OnRightButtonPress += this.UseSecondarySkill;
        InputListener.OnLeftButtonRelease += this.ReleasePrimarySkill;
        InputListener.OnRightButtonRelease += this.ReleaseSecondarySkill;
        InputListener.OnQPress += this.UseSpecialAbility;
        InputListener.OnLeftButtonHold += this.OnLeftMouseButtonHold;
        InputListener.OnRightButtonHold += this.OnRightMouseButtonHold;
        InputListener.OnDelPress += this.KillEnemyAndUnposses;
        InputListener.OnF9Press += this.ToggleAIControl;
        InputListener.OnLeftAltButtonHold += this.HoldAlt;
        InputListener.OnEPress += this.OnInteract;
        this.UpdateComponentsOnCurrentState(true);
    }


    void OnDisable() {
        InputListener.OnNPress -= this.ToggleNoClip;
        InputListener.OnZPress -= this.Unpossess;
        InputListener.OnLeftButtonPress -= this.UsePrimarySkill;
        InputListener.OnRightButtonPress -= this.UseSecondarySkill;
        InputListener.OnLeftButtonRelease -= this.ReleasePrimarySkill;
        InputListener.OnRightButtonRelease -= this.ReleaseSecondarySkill;
        InputListener.OnQPress -= this.UseSpecialAbility;
        InputListener.OnLeftButtonHold -= this.OnLeftMouseButtonHold;
        InputListener.OnRightButtonHold -= this.OnRightMouseButtonHold;
        InputListener.OnDelPress -= this.KillEnemyAndUnposses;
        InputListener.OnF9Press -= this.ToggleAIControl;
        InputListener.OnLeftAltButtonHold -= this.HoldAlt;
        InputListener.OnEPress -= this.OnInteract;
        this.UpdateComponentsOnCurrentState(false);
    }

    void HoldAlt(bool isHeld) => this.IsLeftAltHeld = isHeld;

    void OnLeftMouseButtonHold(bool isPressed) {
        if (isPressed) {
            this.OnPrimarySkillHold();
        }
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

        enemy.TakeOwnerShipIfNotOwned();
        this.UpdateCameraPosition(camera, enemy);
        this.UpdateCameraRotation(camera, enemy);

        if (this.FirstUpdate) {
            this.FirstUpdate = false;
            this.InitCharacterMovement(enemy);
            this.UpdateComponentsOnCurrentState(true);
            this.SetAIControl(false);
        }

        if (this.Controller == null) {
            this.UnregisteredEnemy(enemy);
        }
        else {
            this.CompatibilityMode(localPlayer, enemy, this.Controller, characterMovement, agent);
        }
    }

    void LateUpdate()
    {
        if (Helper.CurrentCamera is not Camera { enabled: true } camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent is not NavMeshAgent agent) return;
        if(this.Controller is not IController controller) return;
        controller.LateUpdate(enemy);
    }

    /// <summary>
    /// This Allows to possess an enemy without a controller Registered to it..
    /// </summary>
    void UnregisteredEnemy(EnemyAI enemy) {
        if (!this.IsAIControlled) {
            this.UpdateEnemyPosition(enemy);
            this.UpdateEnemyRotation();
        }

        if (this.NoClipEnabled) {
            if (this.MainEntrance != null) {
                enemy.SetOutsideStatus(enemy.transform.position.y > this.MainEntrance.transform.position.y + 5.0f);
            }
        }

        if (enemy.isEnemyDead) {
            this.Unpossess();
        }
    }

    /// <summary>
    /// This Allows To possess an Enemy with full compatibility thanks to IController.
    /// </summary>
    void CompatibilityMode(PlayerControllerB player, EnemyAI enemy, IController controller, CharacterMovement movement, NavMeshAgent agent) {
        if (this.NoClipEnabled) {
            if (this.MainEntrance != null) {
                enemy.SetOutsideStatus(enemy.transform.position.y > this.MainEntrance.transform.position.y + 5.0f, controller);
            }
        }

        if (enemy.isEnemyDead) {
            controller.OnDeath(enemy);
            this.Unpossess();
        }

        controller.Update(enemy, this.IsAIControlled);
        player.cursorTip.text = controller.GetPrimarySkillName(enemy);

        if (this.IsAIControlled) {
            return;
        }

        if (controller.SyncAnimationSpeedEnabled(enemy)) {
            movement.CharacterSpeed = agent.speed;
        }

        if (controller.IsAbleToMove(enemy)) {
            movement.CanMove = true;
            controller.OnMovement(enemy, Helper.PlayerInput_isMoving(), Helper.PlayerInput_Sprint());
            this.UpdateEnemyPosition(enemy);
        }
        else {
            movement.CanMove = false;
        }

        if (controller.IsAbleToRotate(enemy)) {
            this.UpdateEnemyRotation();
        }
    }




    void OnInteract() {
        if (this.PossessedEnemy is not EnemyAI enemy) return;
        float maxRange = this.InteractRange(enemy);
        if (maxRange == 0) return;
        Vector3 rayOrigin = enemy.transform.position + new Vector3(0, 0.8f, 0);
        Vector3 rayDirection = enemy.transform.forward;
        int layerMask = 1 << LayerMask.NameToLayer("InteractableObject");

        if (!Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxRange, layerMask)) return;

        if (hit.collider.gameObject.TryGetComponent(out DoorLock doorLock)) {
            this.OpenOrcloseDoorAsEnemy(doorLock);
            return;
        }

        if (this.Controller is not IController controller) return;
        if (!controller.CanUseEntranceDoors(enemy)) return;
        if (hit.collider.gameObject.TryGetComponent(out EntranceTeleport entrance)) {
            this.InteractWithTeleport(enemy, entrance, controller);
        }
    }

    void UpdateCameraPosition(Camera camera, EnemyAI enemy) {
        Vector3 offsets = this.GetCameraOffset();
        Vector3 verticalOffset = offsets.y * Vector3.up;
        Vector3 forwardOffset = offsets.z * camera.transform.forward;
        Vector3 horizontalOffset = offsets.x * camera.transform.right;
        Vector3 offset = horizontalOffset + verticalOffset + forwardOffset;
        camera.transform.position = enemy.transform.position + offset;
    }

    void UpdateCameraRotation(Camera camera, EnemyAI enemy) {
        Quaternion newRotation = !this.IsAIControlled
            ? this.transform.rotation
            : Quaternion.LookRotation(enemy.transform.forward);

        // Set the camera rotation without changing its position
        float RotationLerpSpeed = 0.6f;
        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, newRotation, RotationLerpSpeed);
    }

    void UpdateEnemyRotation() {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (!this.NoClipEnabled) {
            Quaternion horizontalRotation = Quaternion.Euler(0f, this.transform.eulerAngles.y, 0f);
            characterMovement.transform.rotation = horizontalRotation;
        }
        else {
            characterMovement.transform.rotation = this.transform.rotation;
        }
    }

    // Updates enemy's position to match the possessed object's position
    void UpdateEnemyPosition(EnemyAI enemy) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        Vector3 offsets = this.GetEnemyPositionOffset();

        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;

        Vector3 PositionOffset = characterMovement.transform.position + new Vector3(offsets.x, offsets.y, offsets.z);
        enemy.transform.position = PositionOffset;
        if (!this.IsLeftAltHeld) {
            enemy.transform.eulerAngles = enemyEuler;
        }
    }

    // Possesses the specified enemy
    internal void Possess(EnemyAI enemy) {
        if (enemy.isEnemyDead) return;
        this.Unpossess();
        this.Possession.SetEnemy(enemy);
        this.IsAIControlled = false;
        this.InitCharacterMovement(enemy);
        this.FirstUpdate = true;
        enemy.EnableEnemyMesh(true);
        this.Controller = this.GetEnemyController(enemy);
        this.Controller?.OnPossess(enemy);
    }

    void KillEnemyAndUnposses() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (this.Possession.Enemy is not EnemyAI enemy) return;

        enemy.Kill();

        if (localPlayer.IsHost) {
            if (enemy.TryGetComponent(out NetworkObject networkObject)) {
                networkObject.Despawn(true);
            }
        }

        this.Controller?.OnDeath(enemy);
        this.Unpossess();
    }

    // Releases possession of the current enemy
    internal void Unpossess() {
        if (this.Possession.Enemy is EnemyAI enemy) {
            if (enemy.agent is NavMeshAgent agent) {
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.isStopped = false;
                _ = enemy.agent.Warp(enemy.transform.position);
            }
            enemy.SyncPositionToClients();
            this.UpdateEnemyPosition(enemy);
            this.SetAIControl(true);
            this.Controller?.OnUnpossess(enemy);
        }


        this.IsAIControlled = false;
        this.Possession.Clear();
        this.Controller = null;
        if (this.CharacterMovementInstance is not null) {
            Destroy(this.CharacterMovementInstance);
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
        if(this.Controller != null)
        {
            this.Controller.OnEnableAIControl(enemy, enableAI);
        }
    }

    float InteractRange(EnemyAI enemy) => this.Controller?.InteractRange(enemy) ?? IController.DefaultInteractRange;

    float SprintMultiplier(EnemyAI enemy) => this.Controller?.SprintMultiplier(enemy) ?? IController.DefaultSprintMultiplier;


    void OpenOrcloseDoorAsEnemy(DoorLock door) {
        if (door == null) return;
        bool isDoorOpened = door.Reflect().GetInternalField<bool>("isDoorOpened");

        if (door.isLocked) {
            door.UnlockDoorSyncWithServer();
        }
        
        if (isDoorOpened) {
            door.OpenOrCloseDoor(Helper.Players[0]);
        }
        else {
            door.OpenDoorAsEnemyServerRpc();
        }

    }

    Transform? GetExitPointFromDoor(EntranceTeleport entrance) =>
        Helper.FindObjects<EntranceTeleport>().First(teleport =>
            teleport.entranceId == entrance.entranceId && teleport.isEntranceToBuilding != entrance.isEntranceToBuilding
        )?.entrancePoint;

    void InteractWithTeleport(EnemyAI enemy, EntranceTeleport teleport, IController controller) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (this.GetExitPointFromDoor(teleport) is not Transform exitPoint) return;
        characterMovement.SetPosition(exitPoint.position);
        enemy.SetOutsideStatus(!teleport.isEntranceToBuilding, controller);
    }

    Vector3 GetCameraOffset() {
        return this.Possession.Enemy is not EnemyAI enemy || this.Controller is null
            ? IController.DefaultCamOffsets
            : this.Controller.GetCameraOffset(enemy);
    }

    Vector3 GetEnemyPositionOffset() {
        return this.Possession.Enemy is not EnemyAI enemy || this.Controller is null
            ? IController.DefaultEnemyOffset
            : this.Controller.GetEnemyPositionOffset(enemy);
    }



    void UsePrimarySkill() {
        if (this.Possession.Enemy is not EnemyAI enemy || this.Controller is null) return;
        this.Controller.UsePrimarySkill(enemy);
    }

    void UseSecondarySkill() {
        if (this.Possession.Enemy is not EnemyAI enemy || this.Controller is null) return;
        this.Controller.UseSecondarySkill(enemy);
    }
    void OnPrimarySkillHold() {
        if (this.Possession.Enemy is not EnemyAI enemy || this.Controller is null) return;
        this.Controller.OnPrimarySkillHold(enemy);
    }
    void ReleasePrimarySkill() {
        if (this.Possession.Enemy is not EnemyAI enemy || this.Controller is null) return;
        this.Controller.ReleasePrimarySkill(enemy);
    }

    void OnSecondarySkillHold() {
        if (this.Possession.Enemy is not EnemyAI enemy || this.Controller is null) return;
        this.Controller.OnSecondarySkillHold(enemy);
    }


    void ReleaseSecondarySkill() {
        if (this.Possession.Enemy is not EnemyAI enemy || this.Controller is null) return;
        this.Controller.ReleaseSecondarySkill(enemy);
    }

    void UseSpecialAbility() {
        if (this.Possession.Enemy is not EnemyAI enemy || this.Controller is null) return;
        this.Controller.UseSpecialAbility(enemy);
    }
}
