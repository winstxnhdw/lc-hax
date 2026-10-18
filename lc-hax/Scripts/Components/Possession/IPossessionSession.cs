using GameNetcodeStuff;
using UnityEngine;

interface IPossessionSession {
    void Enter(EnemyAI enemy);
    void Tick(EnemyAI enemy, PlayerControllerB player);
    void LateTick(EnemyAI enemy);
    void OnPlayerCollision(Collider other);
}
