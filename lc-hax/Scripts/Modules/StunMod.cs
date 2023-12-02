using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class StunMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;

        if (player == null) {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, 1000f, 524288);

        foreach (Collider collider in colliders) {
            EnemyAICollisionDetect enemy = collider.GetComponent<EnemyAICollisionDetect>();
            enemy?.mainScript.SetEnemyStunned(true, 10.0f, null);
        }
    }
}
