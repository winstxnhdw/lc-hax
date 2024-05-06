namespace Hax;

static partial class Helper {
    
    /// <summary>
    /// Sets the power switch on or off regardless of the BreakerBox state.
    /// </summary>
    /// <param name="On"></param>
    internal static void SetPowerSwitch(bool On)
    {
        if (Helper.RoundManager is not RoundManager roundmanager) return;
        if (On)
        {
            roundmanager.PowerSwitchOnClientRpc();
        }
        else
        {
            roundmanager.PowerSwitchOffClientRpc();
        }

    }

    internal static void FlickerLights(bool flickerFlashlights = false, bool disableFlashlights = false)
    {
        if (Helper.RoundManager is not RoundManager roundmanager) return;
        roundmanager.FlickerLights(flickerFlashlights, disableFlashlights);
    }
}
