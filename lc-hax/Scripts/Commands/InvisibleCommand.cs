using UnityEngine;
using Hax;

[Command("invis")]
class InvisibleCommand : ICommand {
    void ImmediatelyUpdatePlayerPosition() =>
        Helper.LocalPlayer?
              .Reflect()
              .InvokeInternalMethod("UpdatePlayerPositionServerRpc", Vector3.zero, true, true, false, true);

    public void Execute(StringArray _) {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        if (Setting.EnableInvisible) {
            this.ImmediatelyUpdatePlayerPosition();
        }
    }
}
