using Hax;

[Command("triggermod")]
internal class TriggerToggle : ICommand
{
    public void Execute(StringArray _)
    {
        if (TriggerMod.Instance is not TriggerMod triggermod) return;
        triggermod.enabled = !triggermod.enabled;
        Helper.SendFlatNotification(triggermod.enabled ? "Trigger Mod enabled" : "Trigger Mod disabled");
    }
}