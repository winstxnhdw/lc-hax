#region

using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

[Command("noclip")]
class NoClipCommand : ICommand {
    static bool EnabledGodMode { get; set; }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (localPlayer.gameObject.TryGetComponent(out KeyboardMovement keyboard)) {
            Setting.EnableGodMode = EnabledGodMode;
            Object.Destroy(keyboard);
            Chat.Print("NoClip has been disabled!");
        }

        else {
            EnabledGodMode = Setting.EnableGodMode;
            _ = localPlayer.gameObject.AddComponent<KeyboardMovement>();
            Setting.EnableGodMode = true;
            Chat.Print("NoClip has been enabled!");
        }
    }
}
