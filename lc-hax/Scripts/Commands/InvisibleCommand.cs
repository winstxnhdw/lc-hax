using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("invis")]
class InvisibleCommand : ICommand {
    void ImmediatelyUpdatePlayerPosition() =>
        Helper.LocalPlayer?
              .Reflect()
              .InvokeInternalMethod("UpdatePlayerPositionServerRpc", Vector3.zero, true, true, false, true);

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        if (Setting.EnableInvisible) {
            this.ImmediatelyUpdatePlayerPosition();
        }
    }
}
