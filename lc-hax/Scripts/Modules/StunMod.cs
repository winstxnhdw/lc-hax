using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class StunMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = GameNetworkManager.Instance.localPlayerController;

        if (player == null) {
            return;
        }

        Physics.OverlapSphere(player.transform.position, 1000f, 524288)
               .Select(collider => collider.GetComponent<EnemyAICollisionDetect>())
               .Where(enemy => enemy != null)
               .ToList()
               .ForEach(enemy => enemy.mainScript.SetEnemyStunned(true));
    }
}
