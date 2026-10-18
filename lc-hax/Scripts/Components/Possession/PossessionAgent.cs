using UnityEngine;
using UnityEngine.AI;

sealed class PossessionAgent {
    const float NavMeshSearchRadius = 10.0f;

    bool UpdatePosition { get; }
    bool UpdateRotation { get; }
    bool IsStopped { get; }
    Vector3 AgentPosition { get; }
    bool WasOutside { get; }

    internal PossessionAgent(EnemyAI enemy) {
        NavMeshAgent? agent = enemy.agent.Unfake();
        this.WasOutside = enemy.isOutside;
        this.UpdatePosition = agent?.updatePosition ?? false;
        this.UpdateRotation = agent?.updateRotation ?? false;
        this.IsStopped = agent is { enabled: true, isOnNavMesh: true, isStopped: true };
        this.AgentPosition = agent is { enabled: true, isOnNavMesh: true }
            ? agent.nextPosition
            : enemy.transform.position;
    }

    internal void Stop(EnemyAI enemy) {
        enemy.updateDestinationInterval = float.MaxValue;
        this.ConfigureAgent(enemy, updatePosition: false, updateRotation: false, isStopped: true);
    }

    internal void Restore(EnemyAI enemy, IEnemyController controller) {
        enemy.updateDestinationInterval = 0.0f;
        if (enemy.IsOwner) {
            this.RestorePosition(enemy, controller);
        }

        this.ConfigureAgent(enemy, this.UpdatePosition, this.UpdateRotation, this.IsStopped);
    }

    void RestorePosition(EnemyAI enemy, IEnemyController controller) {
        if (enemy.agent is not { enabled: true } agent) return;
        if (enemy is { inSpecialAnimation: true } or { isEnemyDead: true }) return;
        if (agent.Warp(enemy.transform.position)) return;

        if (NavMesh.SamplePosition(
            enemy.transform.position,
            out NavMeshHit hit,
            PossessionAgent.NavMeshSearchRadius,
            agent.areaMask
        )) {
            _ = agent.Warp(hit.position);
            return;
        }

        if (!NavMesh.SamplePosition(
            this.AgentPosition,
            out hit,
            PossessionAgent.NavMeshSearchRadius,
            agent.areaMask
        )) {
            return;
        }

        if (!agent.Warp(hit.position)) {
            return;
        }

        if (enemy.isOutside == this.WasOutside) {
            return;
        }

        enemy.SetOutsideStatus(this.WasOutside);
        controller.OnOutsideStatusChange();
    }

    void ConfigureAgent(EnemyAI enemy, bool updatePosition, bool updateRotation, bool isStopped) {
        NavMeshAgent? agent = enemy.agent.Unfake();
        agent?.updatePosition = updatePosition;
        agent?.updateRotation = updateRotation;

        if (agent is { enabled: true, isOnNavMesh: true } && !enemy.isEnemyDead) {
            agent.isStopped = isStopped;
        }
    }
}
