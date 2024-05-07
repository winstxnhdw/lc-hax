using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("triggermod")]
internal class TriggerToggle : ICommand
{
    public void Execute(StringArray _)
    {
        if (TriggerMod.Instance is not TriggerMod triggermod) return;
        triggermod.enabled = !triggermod.enabled;
        Helper.DisplayFlatHudMessage(triggermod.enabled ? "Trigger Mod enabled" : "Trigger Mod disabled");
    }
}
