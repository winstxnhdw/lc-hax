using Hax;
using UnityEngine;

[Command("invis")]
internal class InvisibleCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        if (Setting.EnableInvisible) ImmediatelyUpdatePlayerPosition();
    }

    private void ImmediatelyUpdatePlayerPosition()
    {
        Helper.LocalPlayer?
            .Reflect()
            .InvokeInternalMethod("UpdatePlayerPositionServerRpc", Vector3.zero, true, true, false, true);
    }
}