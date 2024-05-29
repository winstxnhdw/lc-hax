using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

internal sealed class PossessionMod : MonoBehaviour
{
    private bool IsLeftAltHeld { get; set; } = false;

    internal static PossessionMod? Instance { get; private set; }
    internal bool IsPossessed => Possession.IsPossessed;
    internal EnemyAI? PossessedEnemy => Possession.Enemy;
    internal IController? Controller { get; private set; } = null;

    private Possession Possession { get; } = new();
    private GameObject? CharacterMovementInstance { get; set; } = null;
    private CharacterMovement? CharacterMovement { get; set; } = null;
    private EntranceTeleport? MainEntrance => RoundManager.FindMainEntranceScript(false);

    private MousePan? MousePan { get; set; } = null;

    private bool FirstUpdate { get; set; } = true;
    private bool NoClipEnabled { get; set; } = false;
    private bool IsAIControlled { get; set; } = false;


    // determines the controller to use for each enemy types 
    private Dictionary<Type, IController> EnemyControllers { get; } = new()
    {
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
        { typeof(FlowerSnakeEnemy), new FlowerSnakeController() }
    };

    internal IController? GetEnemyController(EnemyAI enemy)
    {
        return enemy == null ? null : EnemyControllers.GetValueOrDefault(enemy.GetType());
    }

    private void Awake()
    {
        MousePan = gameObject.GetOrAddComponent<MousePan>();
        enabled = false;

        Instance = this;
    }

    private void InitCharacterMovement(EnemyAI? enemy = null)
    {
        CharacterMovementInstance = Finder.Find("Hax CharacterMovement");
        if (CharacterMovementInstance == null)
        {
            CharacterMovementInstance = new GameObject("Hax CharacterMovement");
            CharacterMovementInstance.transform.position = default;
        }

        if (enemy != null)
        {
            CharacterMovementInstance.transform.position = enemy.transform.position;
            CharacterMovement = CharacterMovementInstance.GetOrAddComponent<CharacterMovement>();
            if (CharacterMovement is not null)
            {
                CharacterMovement.SetPosition(enemy.transform.position);
                CharacterMovement.CharacterSprintSpeed = SprintMultiplier(enemy);
                CharacterMovement.CanMove = true;
            }
        }
    }


    private void OnEnable()
    {
        InputListener.OnNPress += ToggleNoClip;
        InputListener.OnZPress += Unpossess;
        InputListener.OnLeftButtonPress += UsePrimarySkill;
        InputListener.OnRightButtonPress += UseSecondarySkill;
        InputListener.OnLeftButtonRelease += ReleasePrimarySkill;
        InputListener.OnRightButtonRelease += ReleaseSecondarySkill;
        InputListener.OnQPress += UseSpecialAbility;
        InputListener.OnLeftButtonHold += OnLeftMouseButtonHold;
        InputListener.OnRightButtonHold += OnRightMouseButtonHold;
        InputListener.OnDelPress += KillEnemyAndUnposses;
        InputListener.OnF9Press += ToggleAIControl;
        InputListener.OnLeftAltButtonHold += HoldAlt;
        InputListener.OnEPress += OnInteract;
        UpdateComponentsOnCurrentState(true);
    }


    private void OnDisable()
    {
        InputListener.OnNPress -= ToggleNoClip;
        InputListener.OnZPress -= Unpossess;
        InputListener.OnLeftButtonPress -= UsePrimarySkill;
        InputListener.OnRightButtonPress -= UseSecondarySkill;
        InputListener.OnLeftButtonRelease -= ReleasePrimarySkill;
        InputListener.OnRightButtonRelease -= ReleaseSecondarySkill;
        InputListener.OnQPress -= UseSpecialAbility;
        InputListener.OnLeftButtonHold -= OnLeftMouseButtonHold;
        InputListener.OnRightButtonHold -= OnRightMouseButtonHold;
        InputListener.OnDelPress -= KillEnemyAndUnposses;
        InputListener.OnF9Press -= ToggleAIControl;
        InputListener.OnLeftAltButtonHold -= HoldAlt;
        InputListener.OnEPress -= OnInteract;
        UpdateComponentsOnCurrentState(false);
    }

    private void HoldAlt(bool isHeld)
    {
        IsLeftAltHeld = isHeld;
    }

    private void OnLeftMouseButtonHold(bool isPressed)
    {
        if (isPressed) OnPrimarySkillHold();
    }

    private void OnRightMouseButtonHold(bool isPressed)
    {
        if (isPressed) OnSecondarySkillHold();
    }

    private void SendPossessionNotifcation(string message)
    {
        Helper.SendNotification(
            "Possession",
            message
        );
    }

    private void ToggleNoClip()
    {
        NoClipEnabled = !NoClipEnabled;
        UpdateComponentsOnCurrentState(enabled);
        SendPossessionNotifcation($"NoClip: {(NoClipEnabled ? "Enabled" : "Disabled")}");
    }

    private void UpdateComponentsOnCurrentState(bool thisGameObjectIsEnabled)
    {
        if (MousePan is null) return;

        MousePan.enabled = thisGameObjectIsEnabled;
        CharacterMovement?.gameObject.SetActive(thisGameObjectIsEnabled);
        CharacterMovement?.SetNoClipMode(NoClipEnabled);
    }

    private void Update()
    {
        if (Helper.CurrentCamera is not Camera { enabled: true } camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (CharacterMovement is not CharacterMovement characterMovement) return;
        if (Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent is not NavMeshAgent agent) return;

        enemy.TakeOwnerShipIfNotOwned();
        UpdateCameraPosition(camera, enemy);
        UpdateCameraRotation(camera, enemy);

        if (FirstUpdate)
        {
            FirstUpdate = false;
            InitCharacterMovement(enemy);
            UpdateComponentsOnCurrentState(true);
            SetAIControl(false);
        }

        if (Controller == null)
            UnregisteredEnemy(enemy);
        else
            CompatibilityMode(localPlayer, enemy, Controller, characterMovement, agent);
    }

    private void LateUpdate()
    {
        if (Helper.CurrentCamera is not Camera { enabled: true } camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (CharacterMovement is not CharacterMovement characterMovement) return;
        if (Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent is not NavMeshAgent agent) return;
        if (Controller is not IController controller) return;
        controller.LateUpdate(enemy);
    }

    /// <summary>
    ///     This Allows to possess an enemy without a controller Registered to it..
    /// </summary>
    private void UnregisteredEnemy(EnemyAI enemy)
    {
        if (!IsAIControlled)
        {
            UpdateEnemyPosition(enemy);
            UpdateEnemyRotation();
        }

        if (NoClipEnabled)
            if (MainEntrance != null)
                enemy.SetOutsideStatus(enemy.transform.position.y > MainEntrance.transform.position.y + 5.0f);

        if (enemy.isEnemyDead) Unpossess();
    }

    /// <summary>
    ///     This Allows To possess an Enemy with full compatibility thanks to IController.
    /// </summary>
    private void CompatibilityMode(PlayerControllerB player, EnemyAI enemy, IController controller,
        CharacterMovement movement, NavMeshAgent agent)
    {
        if (NoClipEnabled)
            if (MainEntrance != null)
                enemy.SetOutsideStatus(enemy.transform.position.y > MainEntrance.transform.position.y + 5.0f,
                    controller);

        if (enemy.isEnemyDead)
        {
            controller.OnDeath(enemy);
            Unpossess();
        }

        controller.Update(enemy, IsAIControlled);
        player.cursorTip.text = controller.GetPrimarySkillName(enemy);

        if (IsAIControlled) return;
        if (controller.SyncAnimationSpeedEnabled(enemy)) movement.CharacterSpeed = agent.speed;

        if (controller.IsAbleToMove(enemy))
        {
            movement.CanMove = true;
            controller.OnMovement(enemy, Helper.PlayerInput_isMoving(), Helper.PlayerInput_Sprint());
            UpdateEnemyPosition(enemy);
        }
        else
        {
            movement.CanMove = false;
        }

        if (controller.IsAbleToRotate(enemy)) UpdateEnemyRotation();
    }


    private void OnInteract()
    {
        if (PossessedEnemy is not EnemyAI enemy) return;
        var maxRange = InteractRange(enemy);
        if (maxRange == 0) return;
        var rayOrigin = enemy.transform.position + new Vector3(0, 0.8f, 0);
        var rayDirection = enemy.transform.forward;
        var layerMask = 1 << LayerMask.NameToLayer("InteractableObject");

        if (!Physics.Raycast(rayOrigin, rayDirection, out var hit, maxRange, layerMask)) return;

        if (hit.collider.gameObject.TryGetComponent(out DoorLock doorLock))
        {
            OpenOrcloseDoorAsEnemy(doorLock);
            return;
        }

        if (Controller is not IController controller) return;
        if (!controller.CanUseEntranceDoors(enemy)) return;
        if (hit.collider.gameObject.TryGetComponent(out EntranceTeleport entrance))
            InteractWithTeleport(enemy, entrance, controller);
    }

    private void UpdateCameraPosition(Camera camera, EnemyAI enemy)
    {
        var offsets = GetCameraOffset();
        var verticalOffset = offsets.y * Vector3.up;
        var forwardOffset = offsets.z * camera.transform.forward;
        var horizontalOffset = offsets.x * camera.transform.right;
        var offset = horizontalOffset + verticalOffset + forwardOffset;
        camera.transform.position = enemy.transform.position + offset;
    }

    private void UpdateCameraRotation(Camera camera, EnemyAI enemy)
    {
        var newRotation = !IsAIControlled
            ? transform.rotation
            : Quaternion.LookRotation(enemy.transform.forward);

        // Set the camera rotation without changing its position
        var RotationLerpSpeed = 0.6f;
        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, newRotation, RotationLerpSpeed);
    }

    private void UpdateEnemyRotation()
    {
        if (CharacterMovement is not CharacterMovement characterMovement) return;
        if (!NoClipEnabled)
        {
            var horizontalRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            characterMovement.transform.rotation = horizontalRotation;
        }
        else
        {
            characterMovement.transform.rotation = transform.rotation;
        }
    }

    // Updates enemy's position to match the possessed object's position
    private void UpdateEnemyPosition(EnemyAI enemy)
    {
        if (CharacterMovement is not CharacterMovement characterMovement) return;
        var offsets = GetEnemyPositionOffset();

        var enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = transform.eulerAngles.y;

        var PositionOffset = characterMovement.transform.position + new Vector3(offsets.x, offsets.y, offsets.z);
        enemy.transform.position = PositionOffset;
        if (!IsLeftAltHeld) enemy.transform.eulerAngles = enemyEuler;
        enemy.SyncPositionToClients();
    }

    // Possesses the specified enemy
    internal void Possess(EnemyAI enemy)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (enemy.isEnemyDead) return;
        if (isHostEnemyOnly(enemy) && !localPlayer.IsHost)
        {
            Helper.SendNotification("Possession", $"This {enemy.enemyType.enemyName} Control is Host Only", true);
            return;
        }

        Unpossess();
        Possession.SetEnemy(enemy);
        IsAIControlled = false;
        InitCharacterMovement(enemy);
        FirstUpdate = true;
        enemy.EnableEnemyMesh(true);
        Controller = GetEnemyController(enemy);
        Controller?.OnPossess(enemy);
    }

    private void KillEnemyAndUnposses()
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Possession.Enemy is not EnemyAI enemy) return;

        enemy.Kill();

        if (localPlayer.IsHost)
            if (enemy.TryGetComponent(out NetworkObject networkObject))
                networkObject.Despawn(true);

        Controller?.OnDeath(enemy);
        Unpossess();
    }

    // Releases possession of the current enemy
    internal void Unpossess()
    {
        if (Possession.Enemy is EnemyAI enemy)
        {
            if (enemy.agent is NavMeshAgent agent)
            {
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.isStopped = false;
                _ = enemy.agent.Warp(enemy.transform.position);
            }

            enemy.SyncPositionToClients();
            UpdateEnemyPosition(enemy);
            SetAIControl(true);
            Controller?.OnUnpossess(enemy);
        }


        IsAIControlled = false;
        Possession.Clear();
        Controller = null;
        if (CharacterMovementInstance is not null) Destroy(CharacterMovementInstance);
    }

    private void ToggleAIControl()
    {
        if (Possession.Enemy?.agent is null) return;
        if (CharacterMovement is null) return;
        if (MousePan is null) return;

        IsAIControlled = !IsAIControlled;
        SetAIControl(IsAIControlled);
        SendPossessionNotifcation($"AI Control: {(IsAIControlled ? "Enabled" : "Disabled")}");
    }

    private void SetAIControl(bool enableAI)
    {
        if (CharacterMovement is not CharacterMovement characterMovement) return;
        if (Possession.Enemy is not EnemyAI enemy) return;
        if (enemy.agent is not NavMeshAgent agent) return;
        if (enableAI)
        {
            _ = enemy.agent.Warp(enemy.transform.position);
            enemy.SyncPositionToClients();
        }

        if (NoClipEnabled)
        {
            NoClipEnabled = false;
            characterMovement.SetNoClipMode(false);
        }

        agent.updatePosition = enableAI;
        agent.updateRotation = enableAI;
        agent.isStopped = !enableAI;
        characterMovement.SetPosition(enemy.transform.position);
        characterMovement.enabled = !enableAI;
        if (Controller != null) Controller.OnEnableAIControl(enemy, enableAI);
    }

    private float InteractRange(EnemyAI enemy)
    {
        return Controller?.InteractRange(enemy) ?? IController.DefaultInteractRange;
    }

    private float SprintMultiplier(EnemyAI enemy)
    {
        return Controller?.SprintMultiplier(enemy) ?? IController.DefaultSprintMultiplier;
    }


    private void OpenOrcloseDoorAsEnemy(DoorLock door)
    {
        if (door == null) return;
        var isDoorOpened = door.Reflect().GetInternalField<bool>("isDoorOpened");

        if (door.isLocked) door.UnlockDoorSyncWithServer();

        if (isDoorOpened)
            door.OpenOrCloseDoor(Helper.Players[0]);
        else
            door.OpenDoorAsEnemyServerRpc();
    }

    private Transform? GetExitPointFromDoor(EntranceTeleport entrance)
    {
        return Helper.FindObjects<EntranceTeleport>().First(teleport =>
            teleport.entranceId == entrance.entranceId && teleport.isEntranceToBuilding != entrance.isEntranceToBuilding
        )?.entrancePoint;
    }

    private void InteractWithTeleport(EnemyAI enemy, EntranceTeleport teleport, IController controller)
    {
        if (CharacterMovement is not CharacterMovement characterMovement) return;
        if (GetExitPointFromDoor(teleport) is not Transform exitPoint) return;
        characterMovement.SetPosition(exitPoint.position);
        enemy.SetOutsideStatus(!teleport.isEntranceToBuilding, controller);
    }

    private Vector3 GetCameraOffset()
    {
        return Possession.Enemy is not EnemyAI enemy || Controller is null
            ? IController.DefaultCamOffsets
            : Controller.GetCameraOffset(enemy);
    }

    private Vector3 GetEnemyPositionOffset()
    {
        return Possession.Enemy is not EnemyAI enemy || Controller is null
            ? IController.DefaultEnemyOffset
            : Controller.GetEnemyPositionOffset(enemy);
    }


    private void UsePrimarySkill()
    {
        if (Possession.Enemy is not EnemyAI enemy || Controller is null) return;
        Controller.UsePrimarySkill(enemy);
    }

    private void UseSecondarySkill()
    {
        if (Possession.Enemy is not EnemyAI enemy || Controller is null) return;
        Controller.UseSecondarySkill(enemy);
    }

    private void OnPrimarySkillHold()
    {
        if (Possession.Enemy is not EnemyAI enemy || Controller is null) return;
        Controller.OnPrimarySkillHold(enemy);
    }

    private void ReleasePrimarySkill()
    {
        if (Possession.Enemy is not EnemyAI enemy || Controller is null) return;
        Controller.ReleasePrimarySkill(enemy);
    }

    private void OnSecondarySkillHold()
    {
        if (Possession.Enemy is not EnemyAI enemy || Controller is null) return;
        Controller.OnSecondarySkillHold(enemy);
    }


    private void ReleaseSecondarySkill()
    {
        if (Possession.Enemy is not EnemyAI enemy || Controller is null) return;
        Controller.ReleaseSecondarySkill(enemy);
    }

    private void UseSpecialAbility()
    {
        if (Possession.Enemy is not EnemyAI enemy || Controller is null) return;
        Controller.UseSpecialAbility(enemy);
    }

    private bool isHostEnemyOnly(EnemyAI enemy)
    {
        if (enemy is null) return false;
        if (GetEnemyController(enemy) is IController controller) return controller.isHostOnly(enemy);
        return false;
    }
}