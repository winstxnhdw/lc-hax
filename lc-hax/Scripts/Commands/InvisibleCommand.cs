#region

using Hax;
using UnityEngine;

#endregion

[Command("invis")]
class InvisibleCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        if (Setting.EnableInvisible) this.ImmediatelyUpdatePlayerPosition();
    }

    void ImmediatelyUpdatePlayerPosition() =>
        Helper.LocalPlayer?
            .Reflect()
            .InvokeInternalMethod("UpdatePlayerPositionServerRpc", Vector3.zero, true, true, false, true);
}
