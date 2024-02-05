using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour {
    public static event Action<bool>? OnShiftButtonHold;
    public static event Action<bool>? OnFButtonHold;
    public static event Action<bool>? OnEButtonHold;
    public static event Action<bool>? OnRButtonHold;
    public static event Action? OnMiddleButtonPress;
    public static event Action? OnLeftButtonPress;
    public static event Action? OnLeftButtonRelease;
    public static event Action? OnRightButtonPress;
    public static event Action? OnRightButtonRelease;
    public static event Action? OnLeftArrowKeyPress;
    public static event Action? OnRightArrowKeyPress;
    public static event Action? OnPausePress;
    public static event Action? OnEqualsPress;
    public static event Action? OnLeftBracketPress;
    public static event Action? OnRightBracketPress;
    public static event Action? OnBackslashPress;
    public static event Action? OnZPress;
    public static event Action? OnXPress;
    public static event Action? OnNPress;
    public static event Action? OnUpArrowPress;
    public static event Action? OnDownArrowPress;
    public static event Action? OnF4Press;
    public static event Action? OnF5Press;
    public static event Action<bool>? OnLeftButtonHold;
    public static event Action<bool>? OnRightButtonHold;

    (Func<bool>, Action)[] InputActions { get; } = [
        (() => Mouse.current.middleButton.wasPressedThisFrame, () => InputListener.OnMiddleButtonPress?.Invoke()),
        (() => Mouse.current.leftButton.wasPressedThisFrame, () => InputListener.OnLeftButtonPress?.Invoke()),
        (() => Mouse.current.leftButton.wasReleasedThisFrame, () => InputListener.OnLeftButtonRelease?.Invoke()),
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
        InputListener.OnRButtonHold?.Invoke(Keyboard.current[Key.R].isPressed);
        InputListener.OnEButtonHold?.Invoke(Keyboard.current[Key.E].isPressed);
        InputListener.OnLeftButtonHold?.Invoke(Mouse.current.leftButton.isPressed);
        InputListener.OnRightButtonHold?.Invoke(Mouse.current.rightButton.isPressed);

        foreach ((Func<bool> keyPressed, Action eventAction) in this.InputActions) {
            if (!keyPressed()) continue;
            eventAction();
        }
    }
}
