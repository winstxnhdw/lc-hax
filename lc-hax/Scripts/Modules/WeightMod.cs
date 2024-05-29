using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class WeightMod : MonoBehaviour
{
    internal static WeightMod? Instance { get; private set; }

    private Coroutine? WeightCoroutine { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private IEnumerator SetWeight(object[] args)
    {
        WaitForEndOfFrame waitForEndOfFrame = new();
        WaitForSeconds waitForOneSecond = new(1.0f);

        while (true)
        {
            if (Helper.LocalPlayer is not PlayerControllerB player)
            {
                yield return waitForEndOfFrame;
                continue;
            }

            player.carryWeight = 1.0f;
            yield return waitForOneSecond;
        }
    }

    internal void StartRoutine()
    {
        WeightCoroutine ??= this.StartResilientCoroutine(SetWeight);
    }

    internal void OnStopRoutine()
    {
        if (WeightCoroutine != null)
        {
            StopCoroutine(WeightCoroutine);
            WeightCoroutine = null;
        }

        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        player.carryWeight = 1.0f;
    }

    public void Start()
    {
        StartRoutine();
    }

    public void OnDisable()
    {
        OnStopRoutine();
    }

    public void OnEnable()
    {
        StartRoutine();
    }

    public void OnDestroy()
    {
        OnStopRoutine();
    }
}