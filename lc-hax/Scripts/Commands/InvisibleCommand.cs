using System;
using UnityEngine;
using Hax;

[Command("/invis")]
public class InvisibleCommand : ICommand {
    void ImmediatelyUpdatePlayerPosition() =>
        Helper.LocalPlayer?
              .Reflect()
              .InvokeInternalMethod("UpdatePlayerPositionServerRpc", Vector3.zero, true, false, true);

    public void Execute(ReadOnlySpan<string> _) {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        if (Setting.EnableInvisible) {
            this.ImmediatelyUpdatePlayerPosition();
        }
    }
}
