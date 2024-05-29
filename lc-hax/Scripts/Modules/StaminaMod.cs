using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class StaminaMod : MonoBehaviour
{
    internal float regenerationRate = 0.1f;
    internal static StaminaMod? Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        player.isSpeedCheating = false;
        player.isSprinting = false;
        player.isExhausted = false;
        player.isMovementHindered = 0;
        player.hinderedMultiplier = 1f;
        player.sourcesCausingSinking = 0;

        if (player.sprintMeter <= 0.2f)
            player.sprintMeter = Mathf.Lerp(player.sprintMeter, 1.0f, Time.deltaTime * regenerationRate);
        else if (player.sprintMeter < 1.0f)
            player.sprintMeter = Mathf.Min(player.sprintMeter + Time.deltaTime * regenerationRate, 1.0f);
    }
}