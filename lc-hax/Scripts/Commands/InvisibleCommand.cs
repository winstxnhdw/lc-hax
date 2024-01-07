using UnityEngine;
using Hax;

[Command("/invis")]
public class InvisibleCommand : ICommand {
    void ImmediatelyUpdatePlayerPosition() =>
        Helper.LocalPlayer?
              .Reflect()
              .InvokeInternalMethod("UpdatePlayerPositionServerRpc", Vector3.zero, true, false, true);

    public void Execute(string[] args) {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        if (Setting.EnableInvisible) {
            this.ImmediatelyUpdatePlayerPosition();
        }
    }
}
