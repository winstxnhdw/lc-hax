using Hax;
using UnityEngine;

internal sealed class DoorTrollMod : MonoBehaviour
{
    internal BreakerBox breaker;
    private float Delay = 0.0f;
    internal float TimeForToggles = 10f;
    internal TerminalAccessibleObject door { get; private set; }

    internal void Start()
    {
        if (!gameObject.GetComponent<TerminalAccessibleObject>())
            Destroy(this);
        door = gameObject.GetComponent<TerminalAccessibleObject>();
        if (!door.isBigDoor)
            Destroy(this);

        if (Helper.BreakerBox is not BreakerBox breakerBox)
            Destroy(this);
        else
            breaker = breakerBox;
    }


    private void Update()
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