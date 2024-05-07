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

    private Coroutine? StaminaCoroutine { get; set; }

    IEnumerator SetSprint(object[] args)
    {
        WaitForEndOfFrame waitForEndOfFrame = new();
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true)
        {
            if (Helper.LocalPlayer is not PlayerControllerB player)
            {
                yield return waitForEndOfFrame;
                continue;
            }

            player.isSpeedCheating = false;
            player.isSprinting = false;
            player.isExhausted = false;
            player.sprintMeter = 1.0f;

            yield return waitForFiveSeconds;
        }
    }

    internal void StartRoutine() => this.StaminaCoroutine ??= this.StartResilientCoroutine(this.SetSprint);

    internal void OnStopRoutine()
    {
        if (this.StaminaCoroutine != null)
        {
            this.StopCoroutine(this.StaminaCoroutine);
            this.StaminaCoroutine = null;
        }
    }

    public void Start() => this.StartRoutine();

    public void OnDisable() => this.OnStopRoutine();

    public void OnEnable() => this.StartRoutine();

    public void OnDestroy() => this.OnStopRoutine();
}