using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class TriggerNodeMod : MonoBehaviour {
    void OnEnable() {
        InputListener.onMiddleButtonPress += this.Fire;
    }

    void OnDisable() {
        InputListener.onMiddleButtonPress -= this.Fire;
    }

    void Fire() {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;
        if (!camera.enabled) return;

        List<RaycastHit> raycastHits = [.. Physics.SphereCastAll(
            camera.transform.position,
            1f,
            camera.transform.forward,
            float.MaxValue
        )];

        raycastHits.ForEach(raycastHit => {
            GameObject gameObject = raycastHit.collider.gameObject;

            if (gameObject.GetComponentInParent<Landmine>().IsNotNull(out Landmine landmine)) {
                _ = Reflector.Target(landmine).InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
                return;
            }

            if (gameObject.GetComponent<JetpackItem>().IsNotNull(out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
            }

            if (gameObject.GetComponent<Turret>().IsNotNull(out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
            }

            if (gameObject.GetComponent<DoorLock>().IsNotNull(out DoorLock doorLock)) {
                doorLock.UnlockDoorSyncWithServer();
            }

            if (gameObject.GetComponent<TerminalAccessibleObject>().IsNotNull(out TerminalAccessibleObject terminalObject)) {
                bool isDoorOpen = Reflector.Target(terminalObject).GetInternalField<bool>("isDoorOpen");
                terminalObject.SetDoorOpenServerRpc(!isDoorOpen);
            }

            if (gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                this.Log($"found {player.playerUsername}");
                if (!Helper.IsNotNull(RoundManager.Instance, out RoundManager roundManager) ||
                    !Helper.IsNotNull(roundManager.SpawnedEnemies, out List<EnemyAI> allEnemies) ||
                    !Helper.IsNotNull(Helper.LocalPlayer, out PlayerControllerB localPlayer) ||
                    player == localPlayer)
                    return;

                allEnemies.ForEach((e) => {
                    e.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
                    e.SetMovingTowardsTargetPlayer(player);
                });

                this.Log($"sending enemies to {player.playerUsername}");
            }
        });
    }
    private void Log(string msg) {
        try {
            Console.Print("RAY", msg);

        }
        catch {

        }
    }
}
