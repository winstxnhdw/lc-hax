using System;
using GameNetcodeStuff;
using UnityEngine;
using Random = UnityEngine.Random;

sealed class SandSpiderController(SandSpiderAI enemy) : IEnemyController {
    SandSpiderAI Enemy { get; } = enemy;

    Vector3 IEnemyController.PositionOffset => Vector3.zero;

    bool IEnemyController.CanMove => true;

    bool IEnemyController.CanRotate => true;

    float IEnemyController.EntranceRange => 2.5f;

    float? IEnemyController.MovementSpeed => this.Enemy.agent.Unfake() is { speed: > 0.0f } agent ? agent.speed : null;

    string? IEnemyController.PrimarySkillName => null;

    string? IEnemyController.SecondarySkillName => null;

    bool IEnemyController.AllowAIToChangeOwner => false;

    float MeshSyncCooldownRemaining { get; set; }

    Vector3 IEnemyController.CameraOffset => new(0.0f, 2.0f, -3.0f);

    void IEnemyController.OnPossess() => this.Enemy.transform.position = this.Enemy.meshContainerPosition;

    void IEnemyController.OnUnpossess(PossessionEndReason reason) => this.Enemy.overrideSpiderLookRotation = false;

    void IEnemyController.UpdateManual() {
        this.Enemy.lookingForWallPosition = false;
        this.Enemy.gotWallPositionInLOS = false;
        this.Enemy.reachedWallPosition = false;
        this.Enemy.onWall = false;
        this.Enemy.overrideSpiderLookRotation = true;
        this.Enemy.meshContainerTargetRotation = Quaternion.LookRotation(this.Enemy.transform.forward);
    }

    void IEnemyController.OnMovement(bool isMoving, bool isSprinting) {
        Vector3 position = this.Enemy.transform.position;
        this.Enemy.creatureAnimator.SetBool("moving", isMoving);
        this.Enemy.meshContainerPosition = position;
        this.Enemy.meshContainerTarget = position;
        this.Enemy.meshContainerTargetRotation = Quaternion.LookRotation(this.Enemy.transform.forward);
        this.Enemy.meshContainer.SetPositionAndRotation(position, this.Enemy.meshContainerTargetRotation);
        this.MeshSyncCooldownRemaining = Mathf.Max(0.0f, this.MeshSyncCooldownRemaining - Time.deltaTime);

        if (this.MeshSyncCooldownRemaining > 0.0f) {
            return;
        }

        this.MeshSyncCooldownRemaining = 0.1f;
        this.Enemy.meshContainerServerPosition = this.Enemy.transform.position;
        this.Enemy.meshContainerServerRotation = this.Enemy.transform.eulerAngles;

        this.Enemy.SyncMeshContainerPositionServerRpc(
            this.Enemy.meshContainerServerPosition,
            this.Enemy.meshContainerServerRotation
        );
    }

    float IEnemyController.SprintMultiplier => 8.0f / 6.0f;

    void IEnemyController.OnOutsideStatusChange() {
        this.Enemy.StopSearch(this.Enemy.patrolHomeBase, true);
        this.Enemy.homeNode = this.Enemy.ChooseClosestNodeToPosition(this.Enemy.transform.position, false, 2);
    }

    void IEnemyController.OnPlayerCollision(PlayerControllerB player) {
        if (this.Enemy.onWall) return;
        if (this.Enemy.spoolingPlayerBody) return;
        if (this.Enemy.timeSinceHittingPlayer <= 1.0f) return;

        this.Enemy.timeSinceHittingPlayer = 0.0f;
        player.DamagePlayerRpc(90);
        this.Enemy.HitPlayerServerRpc(player.PlayerIndex());
    }

    void IEnemyController.OnPrimaryPressed() => this.PlaceWebTrap(this.Enemy);

    void PlaceWebTrap(SandSpiderAI enemy) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;

        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Min(0.0f, randomDirection.y * Random.Range(0.5f, 1.0f));
        Ray ray = new(enemy.abdomen.position + (Vector3.up * 0.4f), randomDirection);
        int collidersAndRoomMask = startOfRound.collidersAndRoomMask;

        if (!Physics.Raycast(ray, out RaycastHit raycastHit, 7.0f, collidersAndRoomMask)) {
            return;
        }

        if (raycastHit.distance < 2.0f) {
            return;
        }

        if (!Physics.Raycast(enemy.abdomen.position, Vector3.down, out RaycastHit groundHit, 10.0f, collidersAndRoomMask)) {
            return;
        }

        Vector3 floorPosition = groundHit.point + (Vector3.up * 0.2f);
        enemy.SpawnWebTrapServerRpc(floorPosition, raycastHit.point);
    }

    void IDisposable.Dispose() { }

    void IEnemyController.Tick(bool inputPaused) { }

    void IEnemyController.OnAIControlChanged(bool isAIControlled) { }

    bool IEnemyController.RequiresServerAI(bool isAIControlled) => false;

    void IEnemyController.OnSecondaryPressed() { }

    void IEnemyController.OnSecondaryHeld() { }

    void IEnemyController.CancelSecondarySkill() { }

    void IEnemyController.ReleaseSecondarySkill() { }
}

