using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;


internal sealed class StaminaMod : MonoBehaviour
{
    internal static StaminaMod? Instance { get; private set; }
    internal float regenerationRate = 0.1f; 

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
        player.isMovementHindered = 0;
        player.hinderedMultiplier = 1f;
        player.sourcesCausingSinking = 0;

        if (player.sprintMeter <= 0.2f)
        {
            player.sprintMeter = Mathf.Lerp(player.sprintMeter, 1.0f, Time.deltaTime * regenerationRate);
        }
        else if (player.sprintMeter < 1.0f)
        {
            player.sprintMeter = Mathf.Min(player.sprintMeter + Time.deltaTime * regenerationRate, 1.0f);
        }

    }
}

    