using Hax;
using UnityEngine;

internal class SandSpiderController : IEnemyController<SandSpiderAI> {
    public void OnMovement(SandSpiderAI enemy, bool isMoving, bool isSprinting) {
        enemy.creatureAnimator.SetBool("moving", true);
        // spider is too slow, make it like 6f default, 8f sprinting
        float speed = isSprinting ? 8.0f : 6.0f;
        enemy.agent.speed = speed;
        enemy.spiderSpeed = speed;
        enemy.SyncMeshContainerPositionToClients();
    }

    public void UsePrimarySkill(SandSpiderAI enemy) => this.PlaceWebTrap(enemy);

    void PlaceWebTrap(SandSpiderAI enemy) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;

        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Min(0.0f, randomDirection.y * Random.Range(0.5f, 1.0f));

        Ray ray = new(enemy.abdomen.position + (Vector3.up * 0.4f), randomDirection);

        if (!Physics.Raycast(ray, out RaycastHit raycastHit, 7.0f, startOfRound.collidersAndRoomMask)) {
            return;
        }

        if (raycastHit.distance < 2.0f) {
            return;
        }

        if (!Physics.Raycast(enemy.abdomen.position, Vector3.down, out RaycastHit groundHit, 10.0f, startOfRound.collidersAndRoomMask)) {
            return;
        }

        Vector3 floorPosition = groundHit.point + (Vector3.up * 0.2f);
        enemy.SpawnWebTrapServerRpc(floorPosition, raycastHit.point);
    }
}

