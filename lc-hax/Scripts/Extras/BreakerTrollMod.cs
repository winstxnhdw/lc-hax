using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

sealed class BreakerTrollMod : MonoBehaviour
{
    internal static BreakerTrollMod? Instance { get; private set; }

    internal BreakerBox Breaker { get; private set; }

    internal InteractTrigger[] Switches { get; private set; }

    internal float TimeBetweenSwitches { get; set; } = 10f;
    private int lastToggledIndex = -1;
    private float nextToggleTime = 0.0f;

    internal void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    internal void Start()
    {
        if(!this.gameObject.GetComponent<BreakerBox>())
            Destroy(this);
        Breaker = this.gameObject.GetComponent<BreakerBox>();
        Switches = Breaker.Get_BreakerBox_Switches();
        if(Switches.Length == 0)
            Destroy(this);
    }


    void Update()
    {
        if(Switches == null || Switches.Length == 0)
            return;

        if(Time.time >= nextToggleTime)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, Switches.Length);
            } while (randomIndex == lastToggledIndex);
            var PickedSwitch = Switches[randomIndex];
            bool isOn = PickedSwitch.Get_BreakerBoxSwitch_State();
            PickedSwitch.Set_BreakerBox_Switch_state(!isOn);
            Helper.SetPowerSwitch(!isOn);


            lastToggledIndex = randomIndex;
            nextToggleTime = Time.time + TimeBetweenSwitches;
        }
    }
}
