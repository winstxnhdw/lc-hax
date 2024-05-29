using System.Linq;

namespace Hax;

internal static partial class Helper
{
    /// <summary>
    ///     Returns the active breakerbox Managing the level's power.
    /// </summary>
    internal static BreakerBox BreakerBox =>
        FindObjects<BreakerBox>().FirstOrDefault(breaker => breaker.name.Contains("(Clone)"));

    /// <summary>
    ///     Sets the power switch on or off regardless of the BreakerBox state.
    /// </summary>
    /// <param name="On"></param>
    internal static void SetPowerSwitch(bool On)
    {
        if (RoundManager is not RoundManager roundmanager) return;
        if (On)
            roundmanager.PowerSwitchOnClientRpc();
        else
            roundmanager.PowerSwitchOffClientRpc();
    }

    internal static void FlickerLights(bool flickerFlashlights = false, bool disableFlashlights = false)
    {
        if (RoundManager is not RoundManager roundmanager) return;
        roundmanager.FlickerLights(flickerFlashlights, disableFlashlights);
    }
}