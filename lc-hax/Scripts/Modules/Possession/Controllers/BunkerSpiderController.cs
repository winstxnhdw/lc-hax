using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal class SandSpiderController : IEnemyController<SandSpiderAI> {
    public void GetCameraPosition(SandSpiderAI enemy) {
        PossessionMod.CamOffsetY = 3f;
        PossessionMod.CamOffsetZ = -3f;
    }

    bool GetOnWall(SandSpiderAI enemy) => enemy.Reflect().GetInternalField<bool>("onWall");

    bool GetSpoolingPlayerBody(SandSpiderAI enemy) =>
        enemy.Reflect().GetInternalField<bool>("spoolingPlayerBody");

    float GetTimeSinceHittingPlayer(SandSpiderAI enemy) =>
        enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SetTimeSinceHittingPlayer(SandSpiderAI enemy, float value) =>
        enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);



    public void Update(SandSpiderAI enemy, bool isAIControlled) {
        enemy.meshContainerPosition = enemy.transform.position;
        enemy.SyncMeshContainerPositionToClients();
        if(!isAIControlled) enemy.homeNode = enemy.ChooseClosestNodeToPosition(enemy.transform.position, false, 2);
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

