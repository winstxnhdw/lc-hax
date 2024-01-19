using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour {
    public static event Action<bool>? onShiftButtonHold;
    public static event Action<bool>? onFButtonHold;
    public static event Action<bool>? onEButtonHold;
    public static event Action<bool>? onRButtonHold;
    public static event Action? onMiddleButtonPress;
    public static event Action? onLeftButtonPress;
    public static event Action? onLeftArrowKeyPress;
    public static event Action? onRightArrowKeyPress;
    public static event Action? onEqualsPress;
    public static event Action? onLeftBracketPress;
    public static event Action? onRightBracketPress;
    public static event Action? onBackslashPress;
    public static event Action? onZPress;
    public static event Action? onXPress;
    public static event Action? onNPress;
    public static event Action? onUpArrowPress;
    public static event Action? onDownArrowPress;

    (Func<bool>, Action)[] InputActions { get; } = [
        (() => Mouse.current.middleButton.wasPressedThisFrame, () => InputListener.onMiddleButtonPress?.Invoke()),
        (() => Mouse.current.leftButton.wasPressedThisFrame, () => InputListener.onLeftButtonPress?.Invoke()),
        (() => true, () => InputListener.onShiftButtonHold?.Invoke(Keyboard.current[Key.LeftShift].isPressed)),
        (() => true, () => InputListener.onFButtonHold?.Invoke(Keyboard.current[Key.F].isPressed)),
        (() => true, () => InputListener.onRButtonHold?.Invoke(Keyboard.current[Key.R].isPressed)),
        (() => true, () => InputListener.onEButtonHold?.Invoke(Keyboard.current[Key.E].isPressed)),
        (() => Keyboard.current[Key.Equals].wasPressedThisFrame, () => InputListener.onEqualsPress?.Invoke()),
        (() => Keyboard.current[Key.LeftArrow].wasPressedThisFrame, () => InputListener.onLeftArrowKeyPress?.Invoke()),
        (() => Keyboard.current[Key.RightArrow].wasPressedThisFrame, () => InputListener.onRightArrowKeyPress?.Invoke()),
        (() => Keyboard.current[Key.LeftBracket].wasPressedThisFrame, () => InputListener.onLeftBracketPress?.Invoke()),
        (() => Keyboard.current[Key.RightBracket].wasPressedThisFrame, () => InputListener.onRightBracketPress?.Invoke()),
        (() => Keyboard.current[Key.Backslash].wasPressedThisFrame, () => InputListener.onBackslashPress?.Invoke()),
        (() => Keyboard.current[Key.Z].wasPressedThisFrame, () => InputListener.onZPress?.Invoke()),
        (() => Keyboard.current[Key.X].wasPressedThisFrame, () => InputListener.onXPress?.Invoke()),
        (() => Keyboard.current[Key.N].wasPressedThisFrame, () => InputListener.onNPress?.Invoke()),
        (() => Keyboard.current[Key.UpArrow].wasPressedThisFrame, () => InputListener.onUpArrowPress?.Invoke()),
        (() => Keyboard.current[Key.DownArrow].wasPressedThisFrame, () => InputListener.onDownArrowPress?.Invoke())
    ];

    void Update() {
        foreach ((Func<bool> keyPressed, Action eventAction) in this.InputActions) {
            if (!keyPressed()) continue;
            eventAction();
        }
    }
}
