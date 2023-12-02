namespace Hax;

public static class Terminal {
    static HUDManager? HUDManager { get; } = HaxObjects.Instance?.HUDManager.Object;

    static Reflector? hudManagerReflector = Terminal.HUDManager == null ? null : Reflector.Target(Terminal.HUDManager);

    static Reflector? HUDManagerReflector {
        get {
            if (Terminal.HUDManager == null) return null;

            hudManagerReflector ??= Reflector.Target(Terminal.HUDManager);
            return hudManagerReflector;
        }
    }

    public static void Print(string name, string message) {
        if (Terminal.HUDManager == null) return;

        _ = Terminal.HUDManagerReflector?.InvokeInternalMethod("AddChatMessage", message, name);

        if (Terminal.HUDManager.localPlayer.isTypingChat) {
            Terminal.HUDManager.localPlayer.isTypingChat = false;
            Terminal.HUDManager.typingIndicator.enabled = false;
            Terminal.HUDManager.chatTextField.text = "";
        }
    }
}



