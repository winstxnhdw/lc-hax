using GameNetcodeStuff;
using System;
using System.Linq;
using UnityEngine;

namespace Hax;

static partial class Helper {

    /// <summary>
    /// This uses the BreakerBox to switch the Breaker on or off switches using the triggers inside it.
    /// </summary>
    /// <param name="breaker"></param>
    /// <param name="On"></param>
    internal static void SetBreakerBox(this BreakerBox breaker, bool On) {
        if (breaker == null) return;
        InteractTrigger[] interactTriggers = breaker.breakerSwitches.Select(breakerSwitch => breakerSwitch.GetComponent<InteractTrigger>()).ToArray();
        if (interactTriggers.Length == 0) return;
        foreach (InteractTrigger interactTrigger in interactTriggers) {
            Animator animator = interactTrigger.GetComponent<Animator>();
            if (animator.GetBool("turnedLeft") == On) continue;
            interactTrigger.Interact(Helper.LocalPlayer?.transform);
        }
    }
}


