using System;
using GameNetcodeStuff;
using UnityEngine;

// Possession runs on non-host clients; actions must use client-callable native paths.
// Dispose restores controller-local and shared state without accessing the enemy.
interface IEnemyController : IDisposable {
    Vector3 CameraOffset { get; }
    Vector3 PositionOffset { get; }
    bool CanMove { get; }
    bool CanRotate { get; }

    // Zero disables automatic entrance use.
    float EntranceRange { get; }
    float SprintMultiplier { get; }

    // Null retains the movement component's current/default speed.
    float? MovementSpeed { get; }

    string? PrimarySkillName { get; }
    string? SecondarySkillName { get; }
    bool AllowAIToChangeOwner { get; }

    // Called once after acquiring a live, spawned enemy owned by this client.
    void OnPossess();

    // The enemy is spawned, but may be dead or owned by another client.
    // Runs before an explicit native kill and before restoring the agent.
    void OnUnpossess(PossessionEndReason reason);

    void OnOutsideStatusChange();

    // The session has already validated the player and collision conditions.
    void OnPlayerCollision(PlayerControllerB player);

    // Runs each session tick, including acquisition, AI mode, and ownership transfers.
    // The enemy is live and spawned; ownership is not guaranteed.
    void Tick(bool inputPaused);

    // Runs only during manual control of a live, spawned enemy owned by this client.
    void UpdateManual();

    // Called once per requested AI/manual mode change, without assuming ownership.
    void OnAIControlChanged(bool isAIControlled);

    bool RequiresServerAI(bool isAIControlled);

    // Input callbacks run during unpaused manual control with a live, spawned enemy
    // owned by this client. Enemy-specific animation and cooldown checks stay here.
    void OnPrimaryPressed();

    void OnSecondaryPressed();

    void OnSecondaryHeld();

    // Clear controller-local input state without accessing the enemy.
    void CancelSecondarySkill();

    // Runs after local cancellation, with a live, spawned enemy owned by this client.
    void ReleaseSecondarySkill();

    // Called after applying manual movement; the enemy is still locally owned.
    void OnMovement(bool isMoving, bool isSprinting);
}
