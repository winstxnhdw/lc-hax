#region

using Hax;
using UnityEngine;

#endregion

[HostCommand("testroom")]
class ToggleTestRoomCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        Setting.isEditorMode = true;
        if (Setting.isEditorMode) {
            bool isTestRoomActive = startOfRound.testRoom != null;
            Helper.SendFlatNotification(isTestRoomActive ? "Test Room disabled" : "Test Room Enabled");
            startOfRound.Debug_EnableTestRoomServerRpc(!isTestRoomActive);
            Helper.Delay(2f, () => { Setting.isEditorMode = false; });
        }
    }
}
