#region

using Hax;
using UnityEngine;

#endregion

sealed class DoorTrollMod : MonoBehaviour {
    internal BreakerBox breaker;
    float Delay = 0.0f;
    internal float TimeForToggles = 10f;
    internal TerminalAccessibleObject door { get; private set; }

    internal void Start() {
        if (!this.gameObject.GetComponent<TerminalAccessibleObject>())
            Destroy(this);
        this.door = this.gameObject.GetComponent<TerminalAccessibleObject>();
        if (!this.door.isBigDoor)
            Destroy(this);

        if (Helper.BreakerBox is not BreakerBox breakerBox)
            Destroy(this);
        else
            this.breaker = breakerBox;
    }


    void Update() {
        if (Helper.RoundManager is not RoundManager round) return;
        if (this.breaker is not BreakerBox breakerBox) return;
        if (this.door == null)
            return;
        if (round.powerOffPermanently) {
            this.door.SetDoor(true);
            Destroy(this);
            return;
        }

        this.Delay += Time.deltaTime;
        if (breakerBox.isPowerOn) {
            if (this.Delay >= this.TimeForToggles) {
                this.door.ToggleDoor();
                this.Delay = 0f;
            }
        }
        else
            Destroy(this);
    }
}
