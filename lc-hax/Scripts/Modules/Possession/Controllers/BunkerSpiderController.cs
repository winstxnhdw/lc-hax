using Hax;
using UnityEngine;

internal class BunkerSpiderController : IEnemyController<SandSpiderAI>
{
    private Vector3 CamOffset { get; } = new(0, 2f, -3f);

    public Vector3 GetCameraOffset(SandSpiderAI _)
    {
        return CamOffset;
    }

    public void OnPossess(SandSpiderAI enemy)
    {
        enemy.transform.position = enemy.meshContainerPosition;
    }

    public void Update(SandSpiderAI enemy, bool isAIControlled)
    {
        enemy.meshContainerPosition = enemy.transform.position;
        var reflect = enemy.Reflect();
        _ = reflect.SetInternalField("overrideSpiderLookRotation", true);
        _ = reflect.SetInternalField("meshContainerTargetRotation", Quaternion.LookRotation(enemy.transform.forward));
        enemy.SyncMeshContainerPositionToClients();
        _ = reflect.SetInternalField("gotWallPositionInLOS", false);
        _ = reflect.SetInternalField("reachedWallPosition", false);
        if (!isAIControlled) enemy.homeNode = enemy.ChooseClosestNodeToPosition(enemy.transform.position, false, 2);
    }

    public bool SyncAnimationSpeedEnabled(SandSpiderAI enemy)
    {
        return false;
    }

    public void UsePrimarySkill(SandSpiderAI enemy)
    {
        PlaceWebTrap(enemy);
    }

    public void OnOutsideStatusChange(SandSpiderAI enemy)
    {
        enemy.StopSearch(enemy.patrolHomeBase, true);
    }

    private void PlaceWebTrap(SandSpiderAI enemy)
    {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;

        var randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Min(0.0f, randomDirection.y * Random.Range(0.5f, 1.0f));

        Ray ray = new(enemy.abdomen.position + Vector3.up * 0.4f, randomDirection);

        if (!Physics.Raycast(ray, out var raycastHit, 7.0f, startOfRound.collidersAndRoomMask)) return;

        if (raycastHit.distance < 2.0f) return;

        if (!Physics.Raycast(enemy.abdomen.position, Vector3.down, out var groundHit, 10.0f,
                startOfRound.collidersAndRoomMask)) return;

        var floorPosition = groundHit.point + Vector3.up * 0.2f;
        enemy.SpawnWebTrapServerRpc(floorPosition, raycastHit.point);
    }
}