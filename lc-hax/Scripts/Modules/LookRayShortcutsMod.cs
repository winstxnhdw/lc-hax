using System.Collections.Generic;
using System.Net;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class LookRayShortcutsMod : MonoBehaviour {
    void OnEnable() {
        InputListener.onMiddleButtonPress += this.Fire;
    }

    void OnDisable() {
        InputListener.onMiddleButtonPress -= this.Fire;
    }

    void Fire() {
        if (!Helper.Extant(Helper.CurrentCamera, out Camera camera)) return;
        if (!camera.enabled) return;

        List<RaycastHit> raycastHits = [.. Physics.SphereCastAll(
            camera.transform.position,
            1f,
            camera.transform.forward,
            float.MaxValue
        )];

        raycastHits.ForEach(raycastHit => {
            GameObject gameObject = raycastHit.collider.gameObject;

            if (Helper.Extant(gameObject.GetComponentInParent<Landmine>(), out Landmine landmine)) {
                _ = Reflector.Target(landmine).InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
                return;
            }

            if (Helper.Extant(gameObject.GetComponent<JetpackItem>(), out JetpackItem jetpack)) {
                jetpack.ExplodeJetpackServerRpc();
            }

            if (Helper.Extant(gameObject.GetComponent<Turret>(), out Turret turret)) {
                turret.EnterBerserkModeServerRpc(-1);
            }

            if (Helper.Extant(gameObject.GetComponent<DoorLock>(), out DoorLock doorLock)) {
                doorLock.UnlockDoorSyncWithServer();
            }

            if (Helper.Extant(gameObject.GetComponent<TerminalAccessibleObject>(), out TerminalAccessibleObject tao)) {
                bool isDoorOpen = Reflector.Target(tao).GetInternalField<bool>("isDoorOpen");
                tao.SetDoorOpenServerRpc(!isDoorOpen);
            }

            if (Helper.Extant(gameObject.GetComponent<PlayerControllerB>(), out PlayerControllerB player)) {
                this.Log($"found {player.playerUsername}");
                if (!Helper.Extant(RoundManager.Instance, out RoundManager roundManager) ||
                    !Helper.Extant(roundManager.SpawnedEnemies, out List<EnemyAI> allEnemies) ||
                    !Helper.Extant(Helper.LocalPlayer, out PlayerControllerB localPlayer) ||
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
