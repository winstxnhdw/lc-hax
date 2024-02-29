using Hax;

[Command("weight")]
internal class WeightToggle : ICommand {
    public void Execute(StringArray _) {
        if (WeightMod.Instance is not WeightMod weight) return;
        weight.enabled = !weight.enabled;
        Helper.SendNotification("Weight Mod", weight.enabled ? " enabled" : "disabled");
    }
}
