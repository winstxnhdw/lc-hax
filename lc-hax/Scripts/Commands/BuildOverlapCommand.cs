using Hax;
using UnityEngine;

[Command("buildoverlap")]
internal class BuildOverlapCommand : ICommand {
    public void Execute(StringArray _) {
        if (ShipBuildModeManager.Instance is not ShipBuildModeManager builder) return;
        Setting.BuildOverlapMode = !Setting.BuildOverlapMode;
        Helper.SendNotification($"Build Overlap ", $"{(Setting.BuildOverlapMode ? "Enabled" : "Disabled")}");
    }

    int GetBuildMask(ShipBuildModeManager instance) => instance.Reflect().GetInternalField<int>("placementMask");

    void SetBuidMask(ShipBuildModeManager instance, int value) =>
        instance.Reflect().SetInternalField("placementMask", value);

    public void AdjustPlacementMaskForColliders(bool ignoreColliders) {
        if (ShipBuildModeManager.Instance is not ShipBuildModeManager builder) return;

        int placementMask = this.GetBuildMask(builder);
        int collidersLayer = LayerMask.NameToLayer("Colliders");

        if (ignoreColliders) {
            // Remove the "Colliders" layer from the placement mask to ignore it
            placementMask &= ~(1 << collidersLayer);
        }
        else {
            // Add the "Colliders" layer back to the placement mask
            placementMask |= (1 << collidersLayer);
        }
        this.SetBuidMask(builder, placementMask);
    }

}
