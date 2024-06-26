#region

using System.Linq;
using GameNetcodeStuff;

#endregion

namespace Hax;

static partial class Helper {
    /// <summary>
    ///     Returns the active breakerbox Managing the level's power.
    /// </summary>
    internal static BreakerBox BreakerBox =>
        FindObjects<BreakerBox>().FirstOrDefault(breaker => breaker.name.Contains("(Clone)"));

    /// <summary>
    ///     Sets the power switch on or off regardless of the BreakerBox state.
    /// </summary>
    /// <param name="On"></param>
    internal static void SetPowerSwitch(bool On) {
        if (RoundManager is not RoundManager roundmanager) return;
        if (On)
            roundmanager.PowerSwitchOnClientRpc();
        else
            roundmanager.PowerSwitchOffClientRpc();
    }

    internal static void FlickerLights(bool flickerFlashlights = false, bool disableFlashlights = false) {
        if (RoundManager is not RoundManager roundmanager) return;
        roundmanager.FlickerLights(flickerFlashlights, disableFlashlights);
    }

    static EntranceTeleport? _InsideMainEntrance;

    internal static EntranceTeleport? InsideMainEntrance {
        get {
            if (Helper.LocalPlayer is not PlayerControllerB
                || Helper.StartOfRound is not StartOfRound startOfRound
                || Helper.RoundManager is not RoundManager roundManager
                || startOfRound.inShipPhase
                || roundManager.currentLevel.levelID == 3) {
                return _InsideMainEntrance = null;
            }

            return _InsideMainEntrance ??= RoundManager.FindMainEntranceScript(true);
        }
    }




}
