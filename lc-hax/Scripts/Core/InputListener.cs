using System;
using UnityEngine;
using UnityEngine.InputSystem;

sealed class InputListener : MonoBehaviour {
    internal static event Action<bool>? OnShiftButtonHold;
    internal static event Action<bool>? OnEButtonHold;
    internal static event Action<bool>? OnWButtonHold;
    internal static event Action<bool>? OnAButtonHold;
    internal static event Action<bool>? OnSButtonHold;
    internal static event Action<bool>? OnDButtonHold;
    internal static event Action<bool>? OnSpaceButtonHold;
    internal static event Action<bool>? OnCtrlButtonHold;
    internal static event Action<bool>? OnRightShiftButtonHold;
    internal static event Action? OnMiddleButtonPress;
    internal static event Action? OnLeftButtonPress;
    internal static event Action? OnRightButtonPress;
    internal static event Action? OnRightButtonRelease;
    internal static event Action? OnLeftArrowKeyPress;
    internal static event Action? OnRightArrowKeyPress;
    internal static event Action? OnPausePress;
    internal static event Action? OnSpacePress;
    internal static event Action<bool>? OnEqualsPress;
    internal static event Action? OnLeftBracketPress;
    internal static event Action? OnRightBracketPress;
    internal static event Action? OnBackslashPress;
    internal static event Action? OnZPress;
    internal static event Action? OnDelPress;
    internal static event Action? OnXPress;
    internal static event Action? OnNPress;
    internal static event Action? OnUpArrowPress;
    internal static event Action? OnDownArrowPress;
    internal static event Action? OnF4Press;
    internal static event Action? OnF5Press;
    internal static event Action? OnF6Press;
    internal static event Action<bool>? OnRightButtonHold;

    void Update() {
        if (Keyboard.current is not { } keyboard) return;
        if (Mouse.current is not { } mouse) return;

        InputListener.OnShiftButtonHold?.Invoke(keyboard[Key.LeftShift].isPressed);
        InputListener.OnEButtonHold?.Invoke(keyboard[Key.E].isPressed);
        InputListener.OnWButtonHold?.Invoke(keyboard[Key.W].isPressed);
        InputListener.OnAButtonHold?.Invoke(keyboard[Key.A].isPressed);
        InputListener.OnSButtonHold?.Invoke(keyboard[Key.S].isPressed);
        InputListener.OnDButtonHold?.Invoke(keyboard[Key.D].isPressed);
        InputListener.OnSpaceButtonHold?.Invoke(keyboard[Key.Space].isPressed);
        InputListener.OnCtrlButtonHold?.Invoke(keyboard.ctrlKey.isPressed);
        InputListener.OnRightShiftButtonHold?.Invoke(keyboard[Key.RightShift].isPressed);
        InputListener.OnRightButtonHold?.Invoke(mouse.rightButton.isPressed);

        if (keyboard[Key.Space].wasPressedThisFrame) {
            InputListener.OnSpacePress?.Invoke();
        }

        if (mouse.middleButton.wasPressedThisFrame) {
            InputListener.OnMiddleButtonPress?.Invoke();
        }

        if (mouse.leftButton.wasPressedThisFrame) {
            InputListener.OnLeftButtonPress?.Invoke();
        }

        if (mouse.rightButton.wasPressedThisFrame) {
            InputListener.OnRightButtonPress?.Invoke();
        }

        if (mouse.rightButton.wasReleasedThisFrame) {
            InputListener.OnRightButtonRelease?.Invoke();
        }

        if (keyboard[Key.Pause].wasPressedThisFrame) {
            InputListener.OnPausePress?.Invoke();
        }

        if (keyboard[Key.Equals].wasPressedThisFrame) {
            InputListener.OnEqualsPress?.Invoke(keyboard[Key.LeftShift].isPressed);
        }

        if (keyboard[Key.LeftArrow].wasPressedThisFrame) {
            InputListener.OnLeftArrowKeyPress?.Invoke();
        }

        if (keyboard[Key.RightArrow].wasPressedThisFrame) {
            InputListener.OnRightArrowKeyPress?.Invoke();
        }

        if (keyboard[Key.LeftBracket].wasPressedThisFrame) {
            InputListener.OnLeftBracketPress?.Invoke();
        }

        if (keyboard[Key.RightBracket].wasPressedThisFrame) {
            InputListener.OnRightBracketPress?.Invoke();
        }

        if (keyboard[Key.Backslash].wasPressedThisFrame) {
            InputListener.OnBackslashPress?.Invoke();
        }

        if (keyboard[Key.Delete].wasPressedThisFrame) {
            InputListener.OnDelPress?.Invoke();
        }

        if (keyboard[Key.Z].wasPressedThisFrame) {
            InputListener.OnZPress?.Invoke();
        }

        if (keyboard[Key.X].wasPressedThisFrame) {
            InputListener.OnXPress?.Invoke();
        }

        if (keyboard[Key.N].wasPressedThisFrame) {
            InputListener.OnNPress?.Invoke();
        }

        if (keyboard[Key.UpArrow].wasPressedThisFrame) {
            InputListener.OnUpArrowPress?.Invoke();
        }

        if (keyboard[Key.DownArrow].wasPressedThisFrame) {
            InputListener.OnDownArrowPress?.Invoke();
        }

        if (keyboard[Key.F4].wasPressedThisFrame) {
            InputListener.OnF4Press?.Invoke();
        }

        if (keyboard[Key.F5].wasPressedThisFrame) {
            InputListener.OnF5Press?.Invoke();
        }

        if (keyboard[Key.F6].wasPressedThisFrame) {
            InputListener.OnF6Press?.Invoke();
        }
    }
}
