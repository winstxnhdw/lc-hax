using Hax;
using UnityEngine;

internal class BunkerSpiderController : IEnemyController<SandSpiderAI> {

    Vector3 CamOffset { get; } = new(0, 2f, -3f);

    public Vector3 GetCameraOffset(SandSpiderAI _) => this.CamOffset;

    public void OnPossess(SandSpiderAI enemy) => enemy.transform.position = enemy.meshContainerPosition;

    public void Update(SandSpiderAI enemy, bool isAIControlled) {
        enemy.meshContainerPosition = enemy.transform.position;
        Reflector<SandSpiderAI> reflect = enemy.Reflect();
        _ = reflect.SetInternalField("overrideSpiderLookRotation", true);
        _ = reflect.SetInternalField("meshContainerTargetRotation", Quaternion.LookRotation(enemy.transform.forward));
        enemy.SyncMeshContainerPositionToClients();
        _ = reflect.SetInternalField("gotWallPositionInLOS", false);
        _ = reflect.SetInternalField("reachedWallPosition", false);
        if (!isAIControlled) enemy.homeNode = enemy.ChooseClosestNodeToPosition(enemy.transform.position, false, 2);
    }

    public bool SyncAnimationSpeedEnabled(SandSpiderAI enemy) => false;

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

    public void OnOutsideStatusChange(SandSpiderAI enemy) => enemy.StopSearch(enemy.patrolHomeBase, true);
}
