#region

using Hax;
using UnityEngine;

#endregion

sealed class BreakerTrollMod : MonoBehaviour {
    float Delay = 0.0f;
    int lastToggledIndex = -1;
    internal static BreakerTrollMod? Instance { get; private set; }

    internal BreakerBox Breaker { get; private set; }

    internal InteractTrigger[] Switches { get; private set; }

    internal float TimeBetweenSwitches { get; set; } = 10f;

    internal void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    internal void Start() {
        if (!this.gameObject.GetComponent<BreakerBox>())
            Destroy(this);
        this.Breaker = this.gameObject.GetComponent<BreakerBox>();
        this.Switches = this.Breaker.Get_BreakerBox_Switches();
        if (this.Switches.Length == 0)
            Destroy(this);
    }


    void Update() {
        if (this.Switches == null || this.Switches.Length == 0)
            return;


        this.Delay += Time.deltaTime;

        if (this.Delay >= this.TimeBetweenSwitches) {
            int randomIndex;
            do
                randomIndex = Random.Range(0, this.Switches.Length);
            while (randomIndex == this.lastToggledIndex);

            InteractTrigger PickedSwitch = this.Switches[randomIndex];
            bool isOn = PickedSwitch.Get_BreakerBoxSwitch_State();
            PickedSwitch.Set_BreakerBox_Switch_state(!isOn);
            Helper.SetPowerSwitch(!isOn);


            this.lastToggledIndex = randomIndex;
            this.Delay = 0f;
        }
    }
}
