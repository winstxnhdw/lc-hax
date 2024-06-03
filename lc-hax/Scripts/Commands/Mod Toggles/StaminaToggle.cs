#region

using Hax;

#endregion

[Command("stamina")]
class StaminaToggle : ICommand {
    public void Execute(StringArray _) {
        if (StaminaMod.Instance is not StaminaMod stamina) return;
        stamina.enabled = !stamina.enabled;
        Helper.SendFlatNotification(stamina.enabled ? "Stamina Mod enabled" : "Stamina Mod disabled");
    }
}
