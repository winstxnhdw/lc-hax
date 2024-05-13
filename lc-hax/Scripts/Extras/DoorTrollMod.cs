using System.Collections;
using GameNetcodeStuff;
using Hax;
using Steamworks.Ugc;
using UnityEngine;

sealed class DoorTrollMod : MonoBehaviour
{
    internal TerminalAccessibleObject door { get; private set; }
    internal float TimeForToggles = 10f;
    private float Delay = 0.0f;
    internal BreakerBox breaker;

    internal void Start()
    {
        if (!this.gameObject.GetComponent<TerminalAccessibleObject>())
            Destroy(this);
        door = this.gameObject.GetComponent<TerminalAccessibleObject>();
        if (!door.isBigDoor)
            Destroy(this);

        if (Helper.BreakerBox is not BreakerBox breakerBox)
        {
            Destroy(this);
        }
        else
        {
            breaker = breakerBox;
        }
    }


    void Update()
    {
        if (Helper.RoundManager is not RoundManager round) return;
        if (breaker is not BreakerBox breakerBox) return;
        if (door == null)
            return;
        if (round.powerOffPermanently)
        {
            door.SetDoor(true);
            Destroy(this);
            return;
        }
        Delay += Time.deltaTime;
        if (breakerBox.isPowerOn)
        {
            if (Delay >= TimeForToggles)
            {
                door.ToggleDoor();
                Delay = 0f;
            }
        }
        else
        {
            Destroy(this);
        }
    }
}
