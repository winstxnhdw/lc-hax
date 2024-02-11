using System;
using UnityEngine;
using UnityEngine.InputSystem;

internal class InputListener : MonoBehaviour {
    internal static event Action<bool>? OnShiftButtonHold;
    internal static event Action<bool>? OnFButtonHold;
    internal static event Action<bool>? OnEButtonHold;
    internal static event Action? OnMiddleButtonPress;
    internal static event Action? OnLeftButtonPress;
    internal static event Action? OnRightButtonPress;
    internal static event Action? OnRightButtonRelease;
    internal static event Action? OnLeftArrowKeyPress;
    internal static event Action? OnRightArrowKeyPress;
    internal static event Action? OnPausePress;
    internal static event Action? OnEqualsPress;
    internal static event Action? OnLeftBracketPress;
    internal static event Action? OnRightBracketPress;
    internal static event Action? OnBackslashPress;
    internal static event Action? OnZPress;
    internal static event Action? OnXPress;
    internal static event Action? OnNPress;
    internal static event Action? OnUpArrowPress;
    internal static event Action? OnDownArrowPress;
    internal static event Action? OnF4Press;
    internal static event Action? OnF5Press;
    internal static event Action<bool>? OnRightButtonHold;

    (Func<bool>, Action)[] InputActions { get; } = [
        (() => Mouse.current.middleButton.wasPressedThisFrame, () => InputListener.OnMiddleButtonPress?.Invoke()),
        (() => Mouse.current.leftButton.wasPressedThisFrame, () => InputListener.OnLeftButtonPress?.Invoke()),
        (() => Mouse.current.rightButton.wasPressedThisFrame, () => InputListener.OnRightButtonPress?.Invoke()),
        (() => Mouse.current.rightButton.wasReleasedThisFrame, () => InputListener.OnRightButtonRelease?.Invoke()),
        (() => Keyboard.current[Key.Pause].wasPressedThisFrame, () => InputListener.OnPausePress?.Invoke()),
        (() => Keyboard.current[Key.Equals].wasPressedThisFrame, () => InputListener.OnEqualsPress?.Invoke()),
        (() => Keyboard.current[Key.LeftArrow].wasPressedThisFrame, () => InputListener.OnLeftArrowKeyPress?.Invoke()),
        (() => Keyboard.current[Key.RightArrow].wasPressedThisFrame, () => InputListener.OnRightArrowKeyPress?.Invoke()),
        (() => Keyboard.current[Key.LeftBracket].wasPressedThisFrame, () => InputListener.OnLeftBracketPress?.Invoke()),
        (() => Keyboard.current[Key.RightBracket].wasPressedThisFrame, () => InputListener.OnRightBracketPress?.Invoke()),
        (() => Keyboard.current[Key.Backslash].wasPressedThisFrame, () => InputListener.OnBackslashPress?.Invoke()),
        (() => Keyboard.current[Key.Z].wasPressedThisFrame, () => InputListener.OnZPress?.Invoke()),
        (() => Keyboard.current[Key.X].wasPressedThisFrame, () => InputListener.OnXPress?.Invoke()),
        (() => Keyboard.current[Key.N].wasPressedThisFrame, () => InputListener.OnNPress?.Invoke()),
        (() => Keyboard.current[Key.UpArrow].wasPressedThisFrame, () => InputListener.OnUpArrowPress?.Invoke()),
        (() => Keyboard.current[Key.DownArrow].wasPressedThisFrame, () => InputListener.OnDownArrowPress?.Invoke()),
        (() => Keyboard.current[Key.F4].wasPressedThisFrame, () => InputListener.OnF4Press?.Invoke()),
        (() => Keyboard.current[Key.F5].wasPressedThisFrame, () => InputListener.OnF5Press?.Invoke()),
    ];

    void Update() {
        InputListener.OnShiftButtonHold?.Invoke(Keyboard.current[Key.LeftShift].isPressed);
        InputListener.OnFButtonHold?.Invoke(Keyboard.current[Key.F].isPressed);
        InputListener.OnEButtonHold?.Invoke(Keyboard.current[Key.E].isPressed);
        InputListener.OnRightButtonHold?.Invoke(Mouse.current.rightButton.isPressed);

        foreach ((Func<bool> keyPressed, Action eventAction) in this.InputActions) {
            if (!keyPressed()) continue;
            eventAction();
        }
    }
}
