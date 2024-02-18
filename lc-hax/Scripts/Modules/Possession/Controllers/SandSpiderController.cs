using Hax;
using UnityEngine;

internal class SandSpiderController : IEnemyController<SandSpiderAI> {


    public void OnMovement(SandSpiderAI enemy, bool isMoving, bool isSprinting) {
        enemy.creatureAnimator.SetBool("moving", true);
        // spider is too slow, make it like 6f default, 8f sprinting
        enemy.agent.speed = isSprinting ? 8f : 6f;
        enemy.spiderSpeed = isSprinting ? 8f : 6f;
        enemy.SyncMeshContainerPositionToClients();
    }

    // primary skill is to drop a web
    public void UsePrimarySkill(SandSpiderAI enemy) => this.PlaceWebTrap(enemy);


    void PlaceWebTrap(SandSpiderAI enemy) {
        if(Helper.StartOfRound is not StartOfRound startOfRound) return; 
        Vector3 randomDirection = Vector3.Scale(UnityEngine.Random.onUnitSphere,
            new Vector3(1f, UnityEngine.Random.Range(0.5f, 1f), 1f));
        randomDirection.y = Mathf.Min(0f, randomDirection.y); 
        Ray ray = new Ray(enemy.abdomen.position + Vector3.up * 0.4f, randomDirection);

        if (Physics.Raycast(ray, out RaycastHit rayHit1, 7f, startOfRound.collidersAndRoomMask)) {
            if (rayHit1.distance < 2f) {
                return; 
            }

            Vector3 point = rayHit1.point; 
            if (Physics.Raycast(enemy.abdomen.position, Vector3.down, out RaycastHit rayHit2, 10f,
                    startOfRound.collidersAndRoomMask)) {
                Vector3 floorPosition = rayHit2.point + Vector3.up * 0.2f; 
                enemy.SpawnWebTrapServerRpc(floorPosition, point); 
            }
        }
    }

}

