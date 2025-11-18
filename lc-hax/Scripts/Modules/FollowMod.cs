using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;
using ZLinq;

struct CopiedStates {
    internal Vector3 position;
    internal Quaternion rotation;
    internal int[] animationStates;
    internal float animationSpeed;
}

sealed class FollowMod : MonoBehaviour {
    internal static PlayerControllerB? PlayerToFollow { get; set; }

    const float SecondsBeforeRealtime = 1.0f;
    const float MaxDistanceFromTarget = 1.0f;

    Queue<CopiedStates> PlayerStates { get; set; } = new();
    Quaternion DeviateRotation { get; set; } = Quaternion.identity;

    float DeviateTimer { get; set; }
    float InstantTeleTimer { get; set; }
    float AnimationBroadcastTimer { get; set; }

    void Update() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (FollowMod.PlayerToFollow is not PlayerControllerB targetPlayer) return;

        if (localPlayer.isPlayerDead || targetPlayer.isPlayerDead) {
            if (FollowMod.PlayerToFollow is not null) {
                FollowMod.PlayerToFollow = null;
                Helper.SendNotification(
                    title: "FollowMod",
                    body: "Following has been disrupted!",
                    isWarning: true
                );
            }

            this.PlayerStates.Clear();
            return;
        }

        localPlayer.ResetFallGravity();
        this.InstantTeleTimer -= Time.deltaTime;

        if (targetPlayer.isClimbingLadder) {
            this.InstantTeleTimer = FollowMod.SecondsBeforeRealtime;
            this.PlayerStates.Clear();
        }

        if (this.InstantTeleTimer > 0.0f) {
            localPlayer.transform.position = targetPlayer.thisPlayerBody.position;
            return;
        }

        this.DeviateTimer -= Time.deltaTime;
        this.AnimationBroadcastTimer -= Time.deltaTime;

        int[] animationStates = [
            .. targetPlayer.playerBodyAnimator
                          .layerCount
                          .Range()
                          .Select(i => targetPlayer.playerBodyAnimator.GetCurrentAnimatorStateInfo(i).fullPathHash)
        ];

        this.PlayerStates.Enqueue(new CopiedStates {
            position = targetPlayer.thisPlayerBody.position.Copy(),
            rotation = targetPlayer.thisPlayerBody.rotation.Copy(),
            animationStates = animationStates,
            animationSpeed = targetPlayer.playerBodyAnimator.GetFloat("animationSpeed")
        });

        //if it isn't time to dequeue data, don't do it.
        if (this.PlayerStates.Count <= SecondsBeforeRealtime / Time.deltaTime) {
            return;
        }

        Quaternion previousRotation = localPlayer.transform.rotation.Copy();
        CopiedStates state = this.PlayerStates.Dequeue();

        localPlayer.transform.rotation = state.rotation * this.DeviateRotation;
        localPlayer.UpdatePlayerRotationServerRpc((short)localPlayer.cameraUp, (short)localPlayer.thisPlayerBody.eulerAngles.y);

        if (this.DeviateTimer < 0) {
            this.DeviateRotation = Quaternion.Euler(0.0f, Random.Range(-360.0f, 360.0f), 0.0f);
            this.DeviateTimer = Random.Range(0.1f, 2.0f);
        }

        localPlayer.transform.rotation = previousRotation;

        // Broadcast copied animation
        if (this.AnimationBroadcastTimer < 0.0f) {
            foreach (int i in state.animationStates.Length.Range()) {
                localPlayer.UpdatePlayerAnimationServerRpc(state.animationStates[i], state.animationSpeed);
            }

            // Too much broadcasting will make your animation stuck on the first animation frame in other player's POV.
            this.AnimationBroadcastTimer = 0.14f;
        }

        if (Vector3.Distance(targetPlayer.thisPlayerBody.position, state.position) < FollowMod.MaxDistanceFromTarget) {
            return;
        }

        localPlayer.transform.position = state.position;
        localPlayer.UpdatePlayerPositionServerRpc(
            localPlayer.thisPlayerBody.localPosition,
            localPlayer.isInElevator,
            localPlayer.isInHangarShipRoom,
            localPlayer.isExhausted,
            localPlayer.thisController.isGrounded
        );
    }
}
