using Unity.Netcode;
using UnityEngine;

sealed class PossessionOwnership {
    const float RetryInterval = 0.5f;

    EnemyAI EnemyReference { get; }
    ulong OriginalOwner { get; }
    ulong? TargetOwner { get; set; }
    float NextRequestTime { get; set; }

    internal PossessionOwnership(EnemyAI enemy) {
        this.EnemyReference = enemy;
        this.OriginalOwner = enemy.OwnerClientId;
    }

    internal bool Request(ulong owner) {
        EnemyAI enemy = this.EnemyReference;

        if (this.TargetOwner != owner) {
            this.TargetOwner = owner;
            this.NextRequestTime = 0.0f;
        }

        if (enemy.OwnerClientId == owner) {
            return true;
        }
        if (Time.unscaledTime < this.NextRequestTime) {
            return false;
        }

        this.NextRequestTime = Time.unscaledTime + PossessionOwnership.RetryInterval;
        enemy.ChangeEnemyOwnerServerRpc(owner);

        return false;
    }

    internal void Restore() {
        if (Helper.StartOfRound is not { } round) return;
        if (this.EnemyReference.Unfake() is not { IsSpawned: true } enemy) return;
        if (!enemy.IsOwner) return;

        ulong owner = round.ClientPlayerList.ContainsKey(this.OriginalOwner) ? this.OriginalOwner : NetworkManager.ServerClientId;

        if (enemy.OwnerClientId != owner) {
            enemy.ChangeEnemyOwnerServerRpc(owner);
        }
    }
}
