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

internal sealed class ScrapPossessionMod : MonoBehaviour {
    bool IsLeftAltHeld { get; set; } = false;

    internal static ScrapPossessionMod? Instance { get; private set; }
    internal bool IsPossessed => this.Possession.IsPossessed;
    internal GrabbableObject? PossessedScrap => this.Possession.Item;
    ScrapPossession Possession { get; } = new();
    GameObject? CharacterMovementInstance { get; set; } = null;
    CharacterMovement? CharacterMovement { get; set; } = null;
    MousePan? MousePan { get; set; } = null;

    bool FirstUpdate { get; set; } = true;
    bool NoClipEnabled { get; set; } = false;

    void Awake() {
        this.InitCharacterMovement();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        this.enabled = false;

        ScrapPossessionMod.Instance = this;
    }

    void InitCharacterMovement(GrabbableObject? item = null) {
        this.CharacterMovement = CharacterMovement.Instance;
        if (this.CharacterMovement == null) {
            GameObject characterMovementObject = new ("Hax CharacterMovement");
            this.CharacterMovement = characterMovementObject.AddComponent<CharacterMovement>();
        }
        this.CharacterMovement.transform.position = item is null ? default : item.transform.position;
        this.CharacterMovement.Init();
        if (item is not null) {
            this.CharacterMovement.CalibrateCollision(item);
            this.CharacterMovement.CharacterSprintSpeed = this.SprintMultiplier();
            this.CharacterMovement.CanMove = true;
        }
    }



    void OnEnable() {
        InputListener.OnNPress += this.ToggleNoClip;
        InputListener.OnZPress += this.Unpossess;
        InputListener.OnLeftButtonPress += this.UsePrimarySkill;
        InputListener.OnRightButtonPress += this.UseSecondarySkill;
        InputListener.OnLeftAltButtonHold += this.HoldAlt;
        InputListener.OnEPress += this.OnInteract;
        this.UpdateComponentsOnCurrentState(true);
    }


    void OnDisable() {
        InputListener.OnNPress -= this.ToggleNoClip;
        InputListener.OnZPress -= this.Unpossess;
        InputListener.OnLeftButtonPress -= this.UsePrimarySkill;
        InputListener.OnRightButtonPress -= this.UseSecondarySkill;
        InputListener.OnLeftAltButtonHold -= this.HoldAlt;
        InputListener.OnEPress -= this.OnInteract;
        this.UpdateComponentsOnCurrentState(false);
    }

    void HoldAlt(bool isHeld) => this.IsLeftAltHeld = isHeld;


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
        if (this.Possession.Item is not GrabbableObject item) return;

        item.ChangeOwnershipOfProp(localPlayer.actualClientId);
        this.UpdateCameraPosition(camera, item);
        this.UpdateCameraRotation(camera);

        if (this.FirstUpdate) {
            this.FirstUpdate = false;
            this.InitCharacterMovement(item);
            this.UpdateComponentsOnCurrentState(true);
            characterMovement.SetPosition(item.transform.position);
        }

        this.UpdateScrapPosition(item);
        this.UpdateScrapRotation();

    }

    void OnInteract() {
        if (this.PossessedScrap is not GrabbableObject item) return;
        float maxRange = this.InteractRange();
        if (maxRange == 0) return;
        Vector3 rayOrigin = item.transform.position + new Vector3(0, 0.8f, 0);
        Vector3 rayDirection = item.transform.forward;
        int layerMask = 1 << LayerMask.NameToLayer("InteractableObject");

        if (!Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxRange, layerMask)) return;

        if (hit.collider.gameObject.TryGetComponent(out DoorLock doorLock)) {
            this.OpenOrcloseDoorAsEnemy(doorLock);
            return;
        }

        if (hit.collider.gameObject.TryGetComponent(out EntranceTeleport entrance)) {
            this.InteractWithTeleport(entrance);
        }
    }

    void UpdateCameraPosition(Camera camera, GrabbableObject item) {
        Vector3 offsets = this.GetCameraOffset();
        Vector3 verticalOffset = offsets.y * Vector3.up;
        Vector3 forwardOffset = offsets.z * camera.transform.forward;
        Vector3 horizontalOffset = offsets.x * camera.transform.right;
        Vector3 offset = horizontalOffset + verticalOffset + forwardOffset;
        camera.transform.position = item.transform.position + offset;
    }

    void UpdateCameraRotation(Camera camera) {
        Quaternion newRotation = this.transform.rotation;

        // Set the camera rotation without changing its position
        float RotationLerpSpeed = 0.6f;
        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, newRotation, RotationLerpSpeed);
    }

    void UpdateScrapRotation() {
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
    void UpdateScrapPosition(GrabbableObject enemy) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        Vector3 offsets = this.GetScrapPositionOffset();

        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;

        Vector3 PositionOffset = characterMovement.transform.position + new Vector3(offsets.x, offsets.y, offsets.z);
        enemy.transform.position = PositionOffset;
        if (!this.IsLeftAltHeld) {
            enemy.transform.eulerAngles = enemyEuler;
        }
    }

    // Possesses the specified enemy
    internal void Possess(GrabbableObject enemy) {
        if (PossessionMod.Instance?.IsPossessed == true) PossessionMod.Instance?.Unpossess();
        this.Unpossess();
        this.FirstUpdate = true;
        this.Possession.SetItem(enemy);
    }


    internal void Unpossess() {
        if (this.Possession.Item is not GrabbableObject item) return;
        this.Possession.Clear();
        if (this.CharacterMovement is not null) {
            Destroy(this.CharacterMovementInstance);
            this.CharacterMovementInstance = null;
        }
    }

    float InteractRange() => 4.5f;

    float SprintMultiplier() => 2.8f;


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

    void InteractWithTeleport(EntranceTeleport teleport) {
        if (this.CharacterMovement is not CharacterMovement characterMovement) return;
        if (this.GetExitPointFromDoor(teleport) is not Transform exitPoint) return;
        characterMovement.SetPosition(exitPoint.position);
    }

    void UsePrimarySkill() {
        if(this.PossessedScrap is not GrabbableObject scrap) return;
        scrap.InteractWithProp();
    }
    void UseSecondarySkill() {
        if (this.PossessedScrap is not GrabbableObject scrap) return;
        scrap.InteractWithProp();

    }


    Vector3 GetCameraOffset() => this.DefaultCamOffsets;
    Vector3 GetScrapPositionOffset() => Vector3.zero;

    Vector3 DefaultCamOffsets => new(0, 2.5f, -3f);

}
