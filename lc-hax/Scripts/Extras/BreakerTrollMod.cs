using Hax;
using UnityEngine;

internal sealed class BreakerTrollMod : MonoBehaviour
{
    private float Delay = 0.0f;
    private int lastToggledIndex = -1;
    internal static BreakerTrollMod? Instance { get; private set; }

    internal BreakerBox Breaker { get; private set; }

    internal InteractTrigger[] Switches { get; private set; }

    internal float TimeBetweenSwitches { get; set; } = 10f;

    internal void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    internal void Start()
    {
        if (!gameObject.GetComponent<BreakerBox>())
            Destroy(this);
        Breaker = gameObject.GetComponent<BreakerBox>();
        Switches = Breaker.Get_BreakerBox_Switches();
        if (Switches.Length == 0)
            Destroy(this);
    }


    private void Update()
    {
        if (Switches == null || Switches.Length == 0)
            return;


        Delay += Time.deltaTime;

        if (Delay >= TimeBetweenSwitches)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, Switches.Length);
            } while (randomIndex == lastToggledIndex);

            var PickedSwitch = Switches[randomIndex];
            var isOn = PickedSwitch.Get_BreakerBoxSwitch_State();
            PickedSwitch.Set_BreakerBox_Switch_state(!isOn);
            Helper.SetPowerSwitch(!isOn);


            lastToggledIndex = randomIndex;
            Delay = 0f;
        }
    }
}