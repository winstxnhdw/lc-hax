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

    static EntranceTeleport _InsideMainEntrance;

    internal static EntranceTeleport? InsideMainEntrance {
        get {
            if (Helper.LocalPlayer is not PlayerControllerB) {
                _InsideMainEntrance = null;
                return null;
            }

            if (Helper.StartOfRound is not StartOfRound round) {
                _InsideMainEntrance = null;
                return null;
            }

            if (round.inShipPhase) {
                _InsideMainEntrance = null;
                return null;
            }

            if (_InsideMainEntrance is not null) {
                return _InsideMainEntrance;
            }

            return _InsideMainEntrance = RoundManager.FindMainEntranceScript(true);
        }
    }


}
