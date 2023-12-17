using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

//in progress
public class FollowAnotherPlayerMod : MonoBehaviour {
    private const float SECOND = 1f;
    struct ValuesToCopy {
        public Vector3 pos;
        public Quaternion rot;
        public int[] animStates;
        public float animSpeed;
    }
    Queue<ValuesToCopy> oneSecondOfValues = new();
    Quaternion deviateRot;
    float deviateTimer = 0;
    float instantTeleTimer = 0;
    float distanceLimit = 1;

    float animBroadcastTimer = 0;
    void Update() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer) ||
            localPlayer.isPlayerDead ||
            !Settings.PlayerToFollow.IsNotNull(out PlayerControllerB player) ||
            player.isPlayerDead) {

            if (Settings.PlayerToFollow.IsNotNull(out PlayerControllerB _)) {
                Console.Print("Stopped following!");
                Settings.PlayerToFollow = null;
            }

            Settings.DisableFallDamage = false;
            this.oneSecondOfValues.Clear();
            return;
        }

        Settings.DisableFallDamage = true;
        this.instantTeleTimer -= Time.deltaTime;

        if (player.isClimbingLadder) {
            this.instantTeleTimer = SECOND;
            this.oneSecondOfValues.Clear();
        }

        if (this.instantTeleTimer > 0) {
            localPlayer.transform.position = player.thisPlayerBody.position;
            return;
        }

        this.deviateTimer -= Time.deltaTime;
        this.animBroadcastTimer -= Time.deltaTime;

        int[] recordAnimStates = new int[player.playerBodyAnimator.layerCount];
        for (int i = 0; i < player.playerBodyAnimator.layerCount; i++) {
            recordAnimStates[i] = player.playerBodyAnimator.GetCurrentAnimatorStateInfo(i).fullPathHash;
        }

        this.oneSecondOfValues.Enqueue(new ValuesToCopy {
            pos = player.thisPlayerBody.position.Copy(),
            rot = player.thisPlayerBody.rotation.Copy(),
            animStates = recordAnimStates,
            animSpeed = player.playerBodyAnimator.GetFloat("animationSpeed")
        });
        //if it isn't time to dequeue data, don't do it.
        if (this.oneSecondOfValues.Count <= SECOND / Time.deltaTime) {
            return;
        }

        ValuesToCopy values = this.oneSecondOfValues.Dequeue();
        Quaternion oldRot = localPlayer.transform.rotation.Copy();
        Vector3 pos = values.pos;
        Quaternion rot = values.rot * this.deviateRot;
        int[] animStates = values.animStates;
        float animSpeed = values.animSpeed;

        localPlayer.transform.rotation = rot;

        //broadcast fake rotation
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

        //broadcast copied animation
        if (this.animBroadcastTimer < 0) {
            for (int i = 0; i < animStates.Length; i++) {
                _ = Reflector.Target(localPlayer)
                .InvokeInternalMethod("UpdatePlayerAnimationServerRpc",
                animStates[i],
                animSpeed);
            };
            //too much broadcast will make your animation stuck at first animation frame in other players pov.
            this.animBroadcastTimer = 0.14f;
        }

        if (Vector3.Distance(player.thisPlayerBody.position, pos) < this.distanceLimit) {
            return;
        }

        localPlayer.transform.position = pos;

        //broadcast copied position.
        _ = Reflector.Target(localPlayer)
        .InvokeInternalMethod("UpdatePlayerPositionServerRpc",
        localPlayer.thisPlayerBody.localPosition,
        localPlayer.isInElevator,
        localPlayer.isExhausted,
        localPlayer.thisController.isGrounded);

    }
}
