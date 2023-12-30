using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

struct CopiedStates {
    public Vector3 position;
    public Quaternion rotation;
    public int[] animationStates;
    public float animationSpeed;
}

public sealed class FollowMod : MonoBehaviour {
    const float secondsBeforeRealtime = 1.0f;
    const float maxDistanceFromTarget = 1.0f;

    Queue<CopiedStates> PlayerStates { get; set; } = new();
    Quaternion DeviateRotation { get; set; } = Quaternion.identity;

    float DeviateTimer { get; set; } = 0.0f;
    float InstantTeleTimer { get; set; } = 0.0f;
    float AnimationBroadcastTimer { get; set; } = 0.0f;

    void Update() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer) || localPlayer.isPlayerDead ||
            !Setting.PlayerToFollow.IsNotNull(out PlayerControllerB player) || player.isPlayerDead
        ) {
            if (Setting.PlayerToFollow is not null) {
                Setting.PlayerToFollow = null;
                Console.Print("Stopped following!");
            }

            Setting.DisableFallDamage = false;
            this.PlayerStates.Clear();
            return;
        }

        Setting.DisableFallDamage = true;
        this.InstantTeleTimer -= Time.deltaTime;

        if (player.isClimbingLadder) {
            this.InstantTeleTimer = FollowMod.secondsBeforeRealtime;
            this.PlayerStates.Clear();
        }

        if (this.InstantTeleTimer > 0.0f) {
            localPlayer.transform.position = player.thisPlayerBody.position;
            return;
        }

        this.DeviateTimer -= Time.deltaTime;
        this.AnimationBroadcastTimer -= Time.deltaTime;

        int[] animationStates =
            player.playerBodyAnimator
                  .layerCount
                  .Range()
                  .Select(i => player.playerBodyAnimator.GetCurrentAnimatorStateInfo(i).fullPathHash).ToArray();

        this.PlayerStates.Enqueue(new CopiedStates {
            position = player.thisPlayerBody.position.Copy(),
            rotation = player.thisPlayerBody.rotation.Copy(),
            animationStates = animationStates,
            animationSpeed = player.playerBodyAnimator.GetFloat("animationSpeed")
        });

        //if it isn't time to dequeue data, don't do it.
        if (this.PlayerStates.Count <= secondsBeforeRealtime / Time.deltaTime) {
            return;
        }

        Quaternion previousRotation = localPlayer.transform.rotation.Copy();
        CopiedStates state = this.PlayerStates.Dequeue();

        localPlayer.transform.rotation = state.rotation * this.DeviateRotation;

        //broadcast fake rotation
        Reflector localPlayerReflector = localPlayer.Reflect();

        _ = localPlayerReflector.InvokeInternalMethod(
            "UpdatePlayerRotationServerRpc",
            (short)localPlayerReflector.GetInternalField<float>("cameraUp"),
            (short)localPlayer.thisPlayerBody.eulerAngles.y
        );

        if (this.DeviateTimer < 0) {
            this.DeviateRotation = Quaternion.Euler(0.0f, Random.Range(-360.0f, 360.0f), 0.0f);
            this.DeviateTimer = Random.Range(0.1f, 2.0f);
        }

        localPlayer.transform.rotation = previousRotation;

        //broadcast copied animation
        if (this.AnimationBroadcastTimer < 0.0f) {
            state.animationStates.Length.Range().ForEach(i => {
                _ = localPlayerReflector.InvokeInternalMethod(
                    "UpdatePlayerAnimationServerRpc",
                    state.animationStates[i],
                    state.animationSpeed
                );
            });

            //too much broadcast will make your animation stuck at first animation frame in other players pov.
            this.AnimationBroadcastTimer = 0.14f;
        }

        if (Vector3.Distance(player.thisPlayerBody.position, state.position) < FollowMod.maxDistanceFromTarget) {
            return;
        }

        localPlayer.transform.position = state.position;

        //broadcast copied position.
        _ = localPlayerReflector.InvokeInternalMethod(
            "UpdatePlayerPositionServerRpc",
            localPlayer.thisPlayerBody.localPosition,
            localPlayer.isInElevator,
            localPlayer.isExhausted,
            localPlayer.thisController.isGrounded
        );
    }
}
