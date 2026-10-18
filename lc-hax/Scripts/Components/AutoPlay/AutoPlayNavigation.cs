using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.AI;

enum AutoPlayMoveResult {
    Moving,
    Arrived,
    Blocked
}

sealed class AutoPlayNavigation(PlayerControllerB player) {
    const float StuckTimeout = 4.5f;

    static int WalkingAreas { get; } = NavMesh.AllAreas &
        ~(1 << NavMesh.GetAreaFromName("Jump")) &
        ~(1 << NavMesh.GetAreaFromName("Climb")) &
        ~(1 << NavMesh.GetAreaFromName("Elevator"));

    PlayerControllerB Player { get; } = player;
    NavMeshPath Path { get; } = new();
    Vector3[] Corners { get; set; } = [];
    Vector3? Destination { get; set; }
    Vector3 ProgressPosition { get; set; }
    int CornerIndex { get; set; }
    float LastProgressTime { get; set; }
    float NextRepathTime { get; set; }
    float NextDoorCheckTime { get; set; }
    int CollisionMask => this.Player.playersManager.collidersAndRoomMaskAndDefault;

    internal void Reset() => this.Destination = null;

    Vector3[] FindPath(Vector3 target) {
        if (!AutoPlayNavigation.TrySample(this.Player.transform.position, 0.8f, 0.6f, out Vector3 source)) return [];
        if (!AutoPlayNavigation.TrySample(target, 1.7f, 1.5f, out Vector3 destination)) return [];

        bool hasPath = NavMesh.CalculatePath(source, destination, AutoPlayNavigation.WalkingAreas, this.Path);
        return hasPath && this.Path.status == NavMeshPathStatus.PathComplete ? this.Path.corners : [];
    }

    internal Vector3? FindDetour() {
        Vector3 position = this.Player.transform.position;

        for (int attempt = 0; attempt < 8; attempt++) {
            Vector2 offset = Random.insideUnitCircle * 6.0f;
            Vector3[] corners = this.FindPath(position + new Vector3(offset.x, 0.0f, offset.y));

            if (corners.Length == 0) {
                continue;
            }

            Vector3 destination = corners[^1];

            if (Vector3.Distance(position, destination) < 2.0f) {
                continue;
            }

            if (NavMesh.Raycast(corners[0], destination, out _, AutoPlayNavigation.WalkingAreas)) {
                continue;
            }

            return destination;
        }

        return null;
    }

    internal AutoPlayMoveResult MoveTo(Vector3 destination, float stoppingDistance, out Vector3 direction) {
        direction = Vector3.zero;
        Vector3 position = this.Player.transform.position;

        if (this.Destination is not Vector3 previous || (destination - previous).sqrMagnitude > 0.25f) {
            this.Destination = destination;
            this.ProgressPosition = position;
            this.LastProgressTime = Time.time;
            this.NextRepathTime = 0.0f;
        }

        if (Time.time >= this.NextRepathTime) {
            this.Corners = this.FindPath(destination);

            if (this.Corners.Length == 0) {
                return AutoPlayMoveResult.Blocked;
            }

            this.CornerIndex = 0;
            this.NextRepathTime = Time.time + 0.75f;
        }

        int lastCornerIndex = this.Corners.Length - 1;

        while (AutoPlayNavigation.IsNear(position, this.Corners[this.CornerIndex],
            this.CornerIndex == lastCornerIndex ? stoppingDistance : 0.3f)) {
            if (this.CornerIndex == lastCornerIndex) return AutoPlayMoveResult.Arrived;

            this.CornerIndex++;
        }

        if (Vector3.Distance(position, this.ProgressPosition) >= 0.2f) {
            this.ProgressPosition = position;
            this.LastProgressTime = Time.time;
        }

        if (Time.time - this.LastProgressTime > AutoPlayNavigation.StuckTimeout) {
            return AutoPlayMoveResult.Blocked;
        }

        Vector3 heading = this.Corners[this.CornerIndex] - position;
        heading.y = 0.0f;
        heading.Normalize();
        this.TryOpenDoor(position);

        bool hasGroundAhead = Physics.Raycast(
            position + (heading * 0.6f) + (Vector3.up * 0.6f),
            Vector3.down,
            1.4f, this.CollisionMask, QueryTriggerInteraction.Ignore
        );

        direction = hasGroundAhead ? heading : Vector3.zero;
        return AutoPlayMoveResult.Moving;
    }

    static bool TrySample(Vector3 position, float radius, float maximumHeight, out Vector3 sampledPosition) {
        bool found = NavMesh.SamplePosition(position, out NavMeshHit hit, radius, AutoPlayNavigation.WalkingAreas);
        sampledPosition = hit.position;
        return found && AutoPlayNavigation.IsNear(position, sampledPosition, 1.0f, maximumHeight);
    }

    static bool IsNear(Vector3 position, Vector3 target, float distance, float height = 0.65f) {
        Vector3 offset = target - position;
        return Mathf.Abs(offset.y) <= height && ((offset.x * offset.x) + (offset.z * offset.z)) <= distance * distance;
    }

    void TryOpenDoor(Vector3 position) {
        if (Time.time < this.NextDoorCheckTime) return;

        this.NextDoorCheckTime = Time.time + 0.25f;
        Vector3 origin = position + Vector3.up;
        int mask = this.CollisionMask | (1 << LayerMask.NameToLayer("InteractableObject"));

        foreach (Collider collider in Physics.OverlapSphere(origin, 2.0f, mask)) {
            if (collider.GetDoor() is not { isLocked: false, isDoorOpened: false } door) continue;

            door.OpenOrCloseDoor(this.Player);
            this.NextDoorCheckTime = Time.time + 1.0f;
            this.NextRepathTime = 0.0f;
            break;
        }
    }
}
