using Hax;

[Command("stamina")]
internal class StaminaToggle : ICommand {
    public void Execute(StringArray _) {
        if (StaminaMod.Instance is not StaminaMod stamina) return;
        stamina.enabled = !stamina.enabled;
        Helper.DisplayFlatHudMessage(stamina.enabled ? "Stamina Mod enabled" : "Stamina Mod disabled");
    }
}
