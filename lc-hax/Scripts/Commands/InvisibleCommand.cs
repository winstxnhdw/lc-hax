using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("invis")]
class InvisibleCommand : ICommand {
    static void ImmediatelyUpdatePlayerPosition() =>
        Helper.LocalPlayer?.UpdatePlayerPositionServerRpc(Vector3.zero, true, true, false, true);

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        if (Setting.EnableInvisible) {
            InvisibleCommand.ImmediatelyUpdatePlayerPosition();
        }
    }
}
