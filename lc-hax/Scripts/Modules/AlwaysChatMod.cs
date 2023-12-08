using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hax;

public class AlwaysChatMod : MonoBehaviour {
    HUDManager hudManager = null;
    PlayerControllerB player = null;

    void Update() {
        this.hudManager = HUDManager.Instance;
        if (this.hudManager == null)
            return;

        this.player = GameNetworkManager.Instance.localPlayerController;
        if (this.player == null)
            return;

        this.hudManager.HideHUD(false);

        var playerActions = this.hudManager.playerActions;

        playerActions.Movement.EnableChat.performed += this.EnableChat;
        playerActions.Movement.SubmitChat.performed += this.SubmitChat;
    }

    void EnableChat(InputAction.CallbackContext context) {
        var prevDeadState = this.player.isPlayerDead;

        this.player.isPlayerDead = false;
        _ = Reflector.Target(this.hudManager).InvokeInternalMethod("EnableChat_performed", context);
        this.player.isPlayerDead = prevDeadState;

    }

    void SubmitChat(InputAction.CallbackContext context) {
        var prevDeadState = this.player.isPlayerDead;

        this.player.isPlayerDead = false;
        _ = Reflector.Target(this.hudManager).InvokeInternalMethod("SubmitChat_performed", context);
        this.player.isPlayerDead = prevDeadState;
    }
}
