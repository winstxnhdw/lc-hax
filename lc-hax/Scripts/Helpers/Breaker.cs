#region

using System.Linq;
using UnityEngine;

#endregion

namespace Hax;

static partial class Helper {
    /// <summary>
    ///     This uses the BreakerBox to switch the Breaker on or off switches using the triggers inside it.
    /// </summary>
    /// <param name="breaker"></param>
    /// <param name="On"></param>
    internal static void Set_All_BreakerBox_Switches(this BreakerBox breaker, bool On) {
        if (breaker == null) return;
        InteractTrigger[] interactTriggers = breaker.Get_BreakerBox_Switches();
        if (interactTriggers.Length == 0) return;
        foreach (InteractTrigger interactTrigger in interactTriggers) interactTrigger.Set_BreakerBox_Switch_state(On);
    }

    internal static InteractTrigger[] Get_BreakerBox_Switches(this BreakerBox breaker) {
        if (breaker == null) return null;
        InteractTrigger[] interactTriggers = breaker.breakerSwitches
            .Select(breakerSwitch => breakerSwitch.GetComponent<InteractTrigger>()).ToArray();
        if (interactTriggers.Length == 0) return null;
        return interactTriggers;
    }

    internal static bool Get_BreakerBoxSwitch_State(this InteractTrigger breakerswitch) {
        if (breakerswitch == null) return false;
        Animator? animator = breakerswitch.GetComponent<Animator>();
        return animator != null && animator.GetBool("turnedLeft");
    }

    internal static void Set_BreakerBox_Switch_state(this InteractTrigger breakerswitch, bool Active) {
        if (breakerswitch == null) return;
        if (breakerswitch.Get_BreakerBoxSwitch_State() == Active) return;
        breakerswitch.Interact(LocalPlayer?.transform);
    }

    // Set a specified BreakerBox switch to a specified state
    internal static void Set_BreakerBox_Switch(this BreakerBox breaker, int index, bool Active) {
        if (breaker == null) return;
        InteractTrigger[] interactTriggers = breaker.Get_BreakerBox_Switches();
        if (interactTriggers.Length == 0) return;
        if (index < 0 || index >= interactTriggers.Length) return;
        interactTriggers[index].Set_BreakerBox_Switch_state(Active);
    }
}
