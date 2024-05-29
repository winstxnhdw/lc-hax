using Hax;

[Command("esp")]
internal class ESPToggle : ICommand
{
    public void Execute(StringArray _)
    {
        if (ESPMod.Instance is not ESPMod esp) return;
        esp.enabled = !esp.enabled;
        Helper.DisplayFlatHudMessage(esp.enabled ? "ESP Mod enabled" : "ESP Mod disabled");
    }
}