using GameNetcodeStuff;
using Hax;
using UnityEngine;

class SandSpiderController : IEnemyController<SandSpiderAI> {
    bool GetOnWall(SandSpiderAI enemy) => enemy.Reflect().GetInternalField<bool>("onWall");

    bool GetSpoolingPlayerBody(SandSpiderAI enemy) =>
        enemy.Reflect().GetInternalField<bool>("spoolingPlayerBody");

    float GetTimeSinceHittingPlayer(SandSpiderAI enemy) =>
        enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SetTimeSinceHittingPlayer(SandSpiderAI enemy, float value) =>
        enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);

    public void Update(SandSpiderAI enemy, bool isAIControlled) {
        enemy.meshContainerPosition = enemy.transform.position;

        public void OnMovement(SandSpiderAI enemy, bool isMoving, bool isSprinting) {
            enemy.creatureAnimator.SetBool("moving", true);
            // spider is too slow, make it like 6f default, 8f sprinting
            float speed = isSprinting ? 8.0f : 6.0f;
            enemy.agent.speed = speed;
            enemy.spiderSpeed = speed;
            enemy.SyncMeshContainerPositionToClients();
            if (!isAIControlled) enemy.homeNode = enemy.ChooseClosestNodeToPosition(enemy.transform.position, false, 2);
        }

        public static bool SyncAnimationSpeedEnabled(SandSpiderAI enemy) {
            return false;
        }

        public void UsePrimarySkill(SandSpiderAI enemy) {
            this.PlaceWebTrap(enemy);
        }

        static void PlaceWebTrap(SandSpiderAI enemy) {
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

        public static void OnOutsideStatusChange(SandSpiderAI enemy) {
            enemy.StopSearch(enemy.patrolHomeBase, true);
        }

        public void OnCollideWithPlayer(SandSpiderAI enemy, PlayerControllerB player) {
            if (enemy.isOutside) {
                if (this.GetOnWall(enemy)) return;
                if (this.GetSpoolingPlayerBody(enemy)) return;
                if (this.GetTimeSinceHittingPlayer(enemy) > 1f) {
                    this.SetTimeSinceHittingPlayer(enemy, 0.0f);
                    player.DamagePlayer(90, true, true, CauseOfDeath.Mauling, 0, false, default);
                    enemy.HitPlayerServerRpc(player.PlayerIndex());
                }
            }
        }
    }

