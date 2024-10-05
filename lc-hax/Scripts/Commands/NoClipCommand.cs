using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("noclip")]
class NoClipCommand : ICommand {
    static bool EnabledGodMode { get; set; }

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (localPlayer.gameObject.TryGetComponent(out KeyboardMovement keyboard)) {
            Setting.EnableGodMode = EnabledGodMode;
            GameObject.Destroy(keyboard);
            Chat.Print("NoClip has been disabled!");
        }

        else {
            NoClipCommand.EnabledGodMode = Setting.EnableGodMode;
            _ = localPlayer.gameObject.AddComponent<KeyboardMovement>();
            Setting.EnableGodMode = true;
            Chat.Print("NoClip has been enabled!");
        }
    }
}
