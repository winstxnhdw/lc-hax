#region

using Hax;
using UnityEngine;

#endregion

[HostCommand("respawnall")]
class RespawnAllCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        Setting.isEditorMode = true;
        if (Setting.isEditorMode) {
            startOfRound.Debug_ReviveAllPlayersServerRpc();
            Helper.SendFlatNotification("Revived All Server Players!");
            Helper.Delay(2f, () => { Setting.isEditorMode = false; });
        }
    }
}
