using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("clearvision")]
internal class ClearVisionToggle : ICommand {
    public void Execute(StringArray _) {
        if (ClearVisionMod.Instance is not ClearVisionMod clearvision) return;
        clearvision.enabled = !clearvision.enabled;
        Helper.SendNotification("ClearVision Mod", clearvision.enabled ? " enabled" : "disabled");
    }
}
