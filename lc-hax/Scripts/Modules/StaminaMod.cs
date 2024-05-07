using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;


internal sealed class StaminaMod : MonoBehaviour
{
    internal static StaminaMod? Instance { get; private set; }

    private void Awake()
    {
        if (StaminaMod.Instance != null)
        {
            Destroy(this);
            return;
        }
        StaminaMod.Instance = this;
    }

    void Update()
    {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        player.isSpeedCheating = false;
        player.isSprinting = false;
        player.isExhausted = false;
        player.sprintMeter = 1.0f;
        player.isMovementHindered = 0;
        player.hinderedMultiplier = 1f;
        player.sourcesCausingSinking = 0;
    }
}

    