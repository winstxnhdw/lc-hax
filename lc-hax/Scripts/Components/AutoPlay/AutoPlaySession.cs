using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;
using ZLinq;

enum AutoPlayState {
    Collecting,
    Returning,
    Waiting
}

sealed class AutoPlaySession : IDisposable {
    const float PickupTimeout = 10.0f;

    internal PlayerControllerB Player { get; }
    internal AutoPlayInput Input { get; }

    AutoPlayNavigation Navigation { get; }
    AutoPlayInventory Inventory { get; }
    HashSet<int> SkippedScrap { get; } = [];
    AutoPlayState State { get; set; }
    GrabbableObject? Scrap { get; set; }
    EntranceTeleport? Entrance { get; set; }
    Vector3? Detour { get; set; }
    bool StayHome { get; set; }
    float PickupWaitTime { get; set; }
    float NextEntranceAttemptTime { get; set; }
    bool WaitingForShipDoor { get; set; }

    internal AutoPlaySession(PlayerControllerB player) {
        this.Player = player;
        this.Input = new AutoPlayInput(player);
        this.Navigation = new AutoPlayNavigation(player);
        this.Inventory = new AutoPlayInventory(player);
        this.ChangeState(AutoPlayState.Collecting);
    }

    static bool IsDepartureNear() {
        if (Helper.TimeOfDay is not { currentDayTimeStarted: true } time) return false;
        if (time.currentLevel is not { planetHasTime: true }) return false;

        float remaining = (time.shipLeaveAutomaticallyTime * time.totalTime) - time.currentDayTime;
        float remainingMinutes = remaining * 60.0f * time.numberOfHours / time.totalTime;

        return remainingMinutes <= Setting.AutoPlayReturnMinutes;
    }

    internal void Tick() {
        this.Input.Stop();
        this.StayHome |= AutoPlaySession.IsDepartureNear();

        if (this.Player.IsInputPaused() || this.Player.inSpecialMenu || this.Inventory.IsBusy) {
            this.Navigation.Reset();
            return;
        }

        if (this.State is AutoPlayState.Waiting) {
            return;
        }

        if (this.State is AutoPlayState.Collecting && (this.StayHome || this.Inventory.ShouldReturn)) {
            this.ChangeState(AutoPlayState.Returning);
        }

        if (this.State is AutoPlayState.Collecting) {
            this.Collect();
            return;
        }

        this.ReturnToShip();
    }

    void ChangeState(AutoPlayState state) {
        this.State = state;
        this.PickupWaitTime = 0.0f;
        this.NextEntranceAttemptTime = 0.0f;
        this.Scrap = null;
        this.Entrance = null;
        this.Detour = null;
        this.WaitingForShipDoor = false;
        this.Navigation.Reset();
    }

    GrabbableObject? ChooseScrap() {
        this.ChangeState(AutoPlayState.Collecting);
        return Helper.Grabbables.AsValueEnumerable()
            .Where(item => this.Inventory.IsCollectible(item) &&
                !this.SkippedScrap.Contains(item.GetInstanceID()))
            .OrderBy(item => item.isInFactory != this.Player.isInsideFactory)
            .ThenBy(item => (item.transform.position - this.Player.transform.position).sqrMagnitude)
            .FirstOrDefault();
    }

    void Collect() {
        this.Scrap = this.Scrap.Unfake() ?? this.ChooseScrap();

        if (this.Scrap is null && this.Player.isInHangarShipRoom && !this.Inventory.HasScrap) {
            return;
        }

        if (this.Scrap is not { } scrap) {
            this.ChangeState(AutoPlayState.Returning);
            return;
        }

        if (!this.Inventory.IsCollectible(scrap)) {
            this.SkipScrap(scrap);
            return;
        }

        if (scrap.isInFactory != this.Player.isInsideFactory) {
            this.UseEntrance();
            return;
        }

        this.Input.LookAt(this.Inventory.GetAimPosition(scrap));

        if (this.Inventory.TryGrab(scrap)) {
            this.SkipScrap(scrap);
            return;
        }

        this.PickupWaitTime += Time.deltaTime;
        AutoPlayMoveResult result = this.Navigation.MoveTo(scrap.transform.position, 0.6f, out Vector3 direction);

        if (result is AutoPlayMoveResult.Blocked || (result is AutoPlayMoveResult.Arrived && this.PickupWaitTime > AutoPlaySession.PickupTimeout)) {
            this.SkipScrap(scrap);
            return;
        }

        if (result is AutoPlayMoveResult.Moving) {
            this.PickupWaitTime = 0.0f;
            this.Input.Move(direction, false);
        }
    }

    void SkipScrap(GrabbableObject scrap) {
        _ = this.SkippedScrap.Add(scrap.GetInstanceID());
        this.ChangeState(AutoPlayState.Collecting);
    }

    void UseEntrance() {
        if (Time.time < this.NextEntranceAttemptTime) return;

        if (this.Entrance == null) {
            this.Entrance = Helper.FindObjects<EntranceTeleport>().AsValueEnumerable()
                .Where(entrance => entrance.Unfake() is not null)
                .Where(entrance => entrance.isEntranceToBuilding != this.Player.isInsideFactory)
                .Where(entrance => entrance.FindExitPoint())
                .FirstOrDefault();

            this.Navigation.Reset();
        }

        if (this.Entrance == null) {
            this.NextEntranceAttemptTime = Time.time + 1.0f;
            return;
        }

        EntranceTeleport selected = this.Entrance;
        AutoPlayMoveResult result = this.WalkTo(this.Detour ?? selected.entrancePoint.position, 1.0f);

        if (result is AutoPlayMoveResult.Blocked) {
            this.Detour = this.Navigation.FindDetour();
            this.NextEntranceAttemptTime = this.Detour is null ? Time.time + 1.0f : 0.0f;
            this.Navigation.Reset();
            return;
        }

        if (result is not AutoPlayMoveResult.Arrived) {
            return;
        }

        if (this.Detour is not null) {
            this.Detour = null;
            this.Navigation.Reset();
            return;
        }

        selected.TeleportPlayer();
        this.Entrance = null;
        this.Navigation.Reset();
    }

    void ReturnToShip() {
        if (this.Player.isInsideFactory) {
            this.UseEntrance();
            return;
        }

        StartOfRound round = this.Player.playersManager;

        if (this.WaitingForShipDoor && round.hangarDoorsClosed) {
            return;
        }

        if (this.WaitingForShipDoor) {
            this.WaitingForShipDoor = false;
            this.Navigation.Reset();
        }

        // The ship's center marker is above the floor; spawn points are at walking height.
        AutoPlayMoveResult result = this.WalkTo(round.playerSpawnPositions[0].position, 0.6f);

        if (result is AutoPlayMoveResult.Blocked &&
            round.hangarDoorsClosed &&
            Vector3.Distance(this.Player.transform.position, round.shipDoorNode.position) < 6.0f
        ) {
            this.WaitingForShipDoor = true;
            return;
        }

        if (result is AutoPlayMoveResult.Blocked) {
            this.Wait("The route to the ship is blocked. Stop AutoPlay to take over.");
            return;
        }

        if (result is AutoPlayMoveResult.Arrived &&
            this.Player.thisController.isGrounded &&
            this.Player.isInElevator &&
            this.Player.isInHangarShipRoom &&
            round.shipInnerRoomBounds.bounds.Contains(this.Player.transform.position + (Vector3.up * 0.25f))
        ) {
            this.Unload(round);
        }
    }

    void Unload(StartOfRound round) {
        if (this.Inventory.HasScrap) {
            this.Input.LookAt(this.Player.transform.position + (this.Player.transform.forward * 0.8f));
            this.Inventory.UnloadNext(round);
            return;
        }

        if (this.StayHome || this.Inventory.ShouldReturn) {
            this.Wait("Waiting in the ship");
            return;
        }

        this.ChangeState(AutoPlayState.Collecting);
    }

    AutoPlayMoveResult WalkTo(Vector3 destination, float distance) {
        AutoPlayMoveResult result = this.Navigation.MoveTo(destination, distance, out Vector3 direction);
        this.Input.Move(direction, this.State is AutoPlayState.Returning && !this.Player.isExhausted);

        if (direction.sqrMagnitude > 0.0f) {
            this.Input.LookAt(this.Player.gameplayCamera.transform.position + (direction * 5.0f));
        }

        return result;
    }

    void Wait(string reason) {
        this.ChangeState(AutoPlayState.Waiting);
        Helper.SendNotification(title: "AutoPlay", body: reason);
    }

    public void Dispose() => this.Input.Release();
}
