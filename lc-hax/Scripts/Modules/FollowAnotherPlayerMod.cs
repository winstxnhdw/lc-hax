using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

//in progress
public class FollowerAnotherPlayerMod : MonoBehaviour {
    Queue<Vector3> oneSecondOfPos = new();
    Queue<Quaternion> oneSecondOfRot = new();
    Quaternion deviateRot;
    float deviateTimer = 0;
    float distanceLimit = 2;
    void Update() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer) ||
            localPlayer.isPlayerDead ||
            !Settings.PlayerToFollow.IsNotNull(out PlayerControllerB player) ||
            player.isPlayerDead) {
            this.oneSecondOfPos.Clear();
            this.oneSecondOfRot.Clear();
            return;
        }
        //prevent fall damage, not working. 
        player.ResetFallGravity();
        player.carryWeight = 0.0f;

        if (player.isClimbingLadder) {
            localPlayer.transform.position = player.thisPlayerBody.position;
            return;
        }

        this.deviateTimer -= Time.deltaTime;

        this.oneSecondOfPos.Enqueue(player.thisPlayerBody.position.Copy());
        this.oneSecondOfRot.Enqueue(player.thisPlayerBody.rotation.Copy());

        //if it isn't time to dequeue data, don't do it.
        if (this.oneSecondOfPos.Count <= 1 / Time.deltaTime) {
            return;
        }

        Quaternion oldRot = localPlayer.transform.rotation.Copy();
        Vector3 pos = this.oneSecondOfPos.Dequeue();
        Quaternion rot = this.oneSecondOfRot.Dequeue() * this.deviateRot;

        localPlayer.transform.rotation = rot;

        _ = Reflector.Target(localPlayer)
        .InvokeInternalMethod("UpdatePlayerRotationServerRpc",
        (short)Reflector.Target(localPlayer).GetInternalField<float>("cameraUp"),
        (short)localPlayer.thisPlayerBody.eulerAngles.y);

        if (this.deviateTimer < 0) {
            this.deviateRot = Quaternion.Euler(
                0,
                Random.Range(-360, 360),
                0
            );
            this.deviateTimer = Random.Range(0.1f, 2f);
        }

        localPlayer.transform.rotation = oldRot;

        if (Vector3.Distance(player.thisPlayerBody.position, pos) < this.distanceLimit) {
            return;
        }

        localPlayer.transform.position = pos;

        _ = Reflector.Target(localPlayer)
        .InvokeInternalMethod("UpdatePlayerPositionServerRpc",
        localPlayer.thisPlayerBody.localPosition,
        localPlayer.isInElevator,
        localPlayer.isExhausted,
        localPlayer.thisController.isGrounded);

    }
}
