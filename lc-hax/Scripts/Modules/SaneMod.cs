using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class SaneMod : MonoBehaviour
{
    internal static SaneMod? Instance { get; private set; }

    private Coroutine? SaneModCoroutine { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        enabled = false;
    }


    private IEnumerator SetSanity(object[] args)
    {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (true)
        {
            if (Helper.LocalPlayer is not PlayerControllerB localPlayer)
            {
                yield return waitForEndOfFrame;
                continue;
            }

            localPlayer.playersManager.gameStats.allPlayerStats[localPlayer.playerClientId].turnAmount = 0;
            localPlayer.playersManager.fearLevel = 0.0f;
            localPlayer.playersManager.fearLevelIncreasing = false;
            localPlayer.insanityLevel = 0.0f;
            localPlayer.insanitySpeedMultiplier = 0.0f;

            yield return waitForEndOfFrame;
        }
    }

    internal void StartRoutine()
    {
        SaneModCoroutine ??= this.StartResilientCoroutine(SetSanity);
    }

    internal void OnStopRoutine()
    {
        if (SaneModCoroutine != null)
        {
            StopCoroutine(SaneModCoroutine);
            SaneModCoroutine = null;
        }
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