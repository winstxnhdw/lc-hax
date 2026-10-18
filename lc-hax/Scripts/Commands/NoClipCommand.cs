using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("noclip")]
sealed class NoClipCommand : ICommand {
    static bool EnabledGodMode { get; set; }

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { } localPlayer) return Task.CompletedTask;
        if (localPlayer.gameObject.TryGetComponent(out KeyboardMovement keyboard)) {
            Globals.EnableGodMode = EnabledGodMode;
            GameObject.Destroy(keyboard);
            Chat.Print("NoClip has been disabled!");
        }

        else {
            NoClipCommand.EnabledGodMode = Globals.EnableGodMode;
            _ = localPlayer.gameObject.AddComponent<KeyboardMovement>();
            Globals.EnableGodMode = true;
            Chat.Print("NoClip has been enabled!");
        }

        return Task.CompletedTask;
    }
}
