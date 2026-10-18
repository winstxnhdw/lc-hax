using UnityEngine;

sealed class PossessionEntrances(EnemyAI enemy, IEnemyController controller, PossessionMotor motor) {
    const float TeleportDoorCooldown = 2.5f;
    const float InteractionOriginHeight = 0.8f;
    const float MinimumEntranceHeightSeparation = 50.0f;

    EnemyAI Enemy { get; } = enemy;
    IEnemyController Controller { get; } = controller;
    PossessionMotor Motor { get; } = motor;
    EntranceTeleport? InsideEntrance { get; } = RoundManager.FindMainEntranceScript(false);
    EntranceTeleport? OutsideEntrance { get; } = RoundManager.FindMainEntranceScript(true);
    float TeleportCooldownRemaining { get; set; }

    internal void Tick() => this.TeleportCooldownRemaining = Mathf.Max(
        0.0f,
        this.TeleportCooldownRemaining - Time.deltaTime
    );

    internal bool TryUseEntrance() {
        if (this.Enemy.inSpecialAnimation) return false;
        if (this.TeleportCooldownRemaining > 0.0f) return false;
        if (this.Controller.EntranceRange <= 0.0f) return false;

        bool hasHit = Physics.Raycast(
            this.Enemy.transform.position + (Vector3.up * PossessionEntrances.InteractionOriginHeight),
            this.Enemy.transform.forward,
            out RaycastHit hit,
            this.Controller.EntranceRange,
            1 << LayerMask.NameToLayer("InteractableObject"),
            QueryTriggerInteraction.Collide
        );

        return
            hasHit &&
            hit.collider.GetComponentInParent<EntranceTeleport>().Unfake() is EntranceTeleport entrance &&
            this.InteractWithTeleport(entrance);
    }

    bool InteractWithTeleport(EntranceTeleport entrance) {
        if (!entrance.FindExitPoint()) return false;

        Vector3 position = entrance.exitScript.entrancePoint.position;
        this.Motor.SetPosition(position);
        this.Enemy.transform.position = position;
        this.SetOutsideStatus(!entrance.isEntranceToBuilding);
        this.Enemy.EnableEnemyMesh(true, false);
        this.TeleportCooldownRemaining = PossessionEntrances.TeleportDoorCooldown;

        return true;
    }

    internal void UpdateOutsideStatus() {
        if (!this.Motor.NoClipEnabled) return;
        if (this.InsideEntrance == null) return;
        if (this.OutsideEntrance == null) return;

        float insideHeight = this.InsideEntrance.entrancePoint.position.y;
        float outsideHeight = this.OutsideEntrance.entrancePoint.position.y;

        if (Mathf.Abs(outsideHeight - insideHeight) < PossessionEntrances.MinimumEntranceHeightSeparation) {
            return;
        }

        float height = this.Enemy.transform.position.y;
        float insideDistance = Mathf.Abs(height - insideHeight);
        float outsideDistance = Mathf.Abs(height - outsideHeight);
        this.SetOutsideStatus(outsideDistance < insideDistance);
    }

    void SetOutsideStatus(bool isOutside) {
        if (this.Enemy.isOutside == isOutside) return;

        this.Enemy.SetOutsideStatus(isOutside);
        this.Controller.OnOutsideStatusChange();
    }
}
