using Hax;
using UnityEngine;

class BunkerSpiderController : IEnemyController<SandSpiderAI> {
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

    public Vector3 GetCameraOffset(SandSpiderAI _) => new(0.0f, 2.0f, -3.0f);

    public void OnPossess(SandSpiderAI enemy) => enemy.transform.position = enemy.meshContainerPosition;

    public void Update(SandSpiderAI enemy, bool isAIControlled) {
        if (!isAIControlled) {
            enemy.homeNode = enemy.ChooseClosestNodeToPosition(enemy.transform.position, false, 2);
        }

        _ = enemy.Reflect()
                 .SetInternalField("overrideSpiderLookRotation", true)?
                 .SetInternalField("meshContainerTargetRotation", Quaternion.LookRotation(enemy.transform.forward))?
                 .SetInternalField("gotWallPositionInLOS", false)?
                 .SetInternalField("reachedWallPosition", false);

        enemy.meshContainerPosition = enemy.transform.position;
        enemy.SyncMeshContainerPositionToClients();
    }

    public bool SyncAnimationSpeedEnabled(SandSpiderAI enemy) => false;

    public void UsePrimarySkill(SandSpiderAI enemy) => this.PlaceWebTrap(enemy);

    public void OnOutsideStatusChange(SandSpiderAI enemy) => enemy.StopSearch(enemy.patrolHomeBase, true);
}
