using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal struct CopiedStates
{
    internal Vector3 position;
    internal Quaternion rotation;
    internal int[] animationStates;
    internal float animationSpeed;
}

internal sealed class FollowMod : MonoBehaviour
{
    private const float SecondsBeforeRealtime = 1.0f;
    private const float MaxDistanceFromTarget = 1.0f;
    internal static PlayerControllerB? PlayerToFollow { get; set; }

    private Queue<CopiedStates> PlayerStates { get; } = new();
    private Quaternion DeviateRotation { get; set; } = Quaternion.identity;

    private float DeviateTimer { get; set; } = 0.0f;
    private float InstantTeleTimer { get; set; } = 0.0f;
    private float AnimationBroadcastTimer { get; set; } = 0.0f;

    private void Update()
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (PlayerToFollow is not PlayerControllerB targetPlayer) return;

        if (localPlayer.isPlayerDead || targetPlayer.isPlayerDead)
        {
            if (PlayerToFollow is not null)
            {
                PlayerToFollow = null;
                Helper.SendNotification(
                    "FollowMod",
                    "Following has been disrupted!",
                    true
                );
            }

            PlayerStates.Clear();
            return;
        }

        localPlayer.ResetFallGravity();
        InstantTeleTimer -= Time.deltaTime;

        if (targetPlayer.isClimbingLadder)
        {
            InstantTeleTimer = SecondsBeforeRealtime;
            PlayerStates.Clear();
        }

        if (InstantTeleTimer > 0.0f)
        {
            localPlayer.transform.position = targetPlayer.thisPlayerBody.position;
            return;
        }

        DeviateTimer -= Time.deltaTime;
        AnimationBroadcastTimer -= Time.deltaTime;

        var animationStates =
            targetPlayer.playerBodyAnimator
                .layerCount
                .Range()
                .Select(i => targetPlayer.playerBodyAnimator.GetCurrentAnimatorStateInfo(i).fullPathHash)
                .ToArray();

        PlayerStates.Enqueue(new CopiedStates
        {
            position = targetPlayer.thisPlayerBody.position.Copy(),
            rotation = targetPlayer.thisPlayerBody.rotation.Copy(),
            animationStates = animationStates,
            animationSpeed = targetPlayer.playerBodyAnimator.GetFloat("animationSpeed")
        });

        //if it isn't time to dequeue data, don't do it.
        if (PlayerStates.Count <= SecondsBeforeRealtime / Time.deltaTime) return;

        var previousRotation = localPlayer.transform.rotation.Copy();
        var state = PlayerStates.Dequeue();

        localPlayer.transform.rotation = state.rotation * DeviateRotation;

        //broadcast fake rotation
        var localPlayerReflector = localPlayer.Reflect();

        _ = localPlayerReflector.InvokeInternalMethod(
            "UpdatePlayerRotationServerRpc",
            (short)localPlayerReflector.GetInternalField<float>("cameraUp"),
            (short)localPlayer.thisPlayerBody.eulerAngles.y
        );

        if (DeviateTimer < 0)
        {
            DeviateRotation = Quaternion.Euler(0.0f, Random.Range(-360.0f, 360.0f), 0.0f);
            DeviateTimer = Random.Range(0.1f, 2.0f);
        }

        localPlayer.transform.rotation = previousRotation;

        //broadcast copied animation
        if (AnimationBroadcastTimer < 0.0f)
        {
            state.animationStates.Length.Range().ForEach(i =>
            {
                _ = localPlayerReflector.InvokeInternalMethod(
                    "UpdatePlayerAnimationServerRpc",
                    state.animationStates[i],
                    state.animationSpeed
                );
            });

            //too much broadcast will make your animation stuck at first animation frame in other players pov.
            AnimationBroadcastTimer = 0.14f;
        }

        if (Vector3.Distance(targetPlayer.thisPlayerBody.position, state.position) < MaxDistanceFromTarget) return;

        localPlayer.transform.position = state.position;

        //broadcast copied position.
        _ = localPlayerReflector.InvokeInternalMethod(
            "UpdatePlayerPositionServerRpc",
            localPlayer.thisPlayerBody.localPosition,
            localPlayer.isInElevator,
            localPlayer.isInHangarShipRoom,
            localPlayer.isExhausted,
            localPlayer.thisController.isGrounded
        );
    }
}