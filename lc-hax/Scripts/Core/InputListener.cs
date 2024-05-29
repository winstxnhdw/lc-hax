using System;
using Hax;
using UnityEngine;
using UnityEngine.InputSystem;

internal class InputListener : MonoBehaviour
{
    private (Func<bool>, Action)[] InputActions { get; } =
    [
        (() => Mouse.current.middleButton.wasPressedThisFrame, () => OnMiddleButtonPress?.Invoke()),
        (() => Mouse.current.leftButton.wasPressedThisFrame, () => OnLeftButtonPress?.Invoke()),
        (() => Mouse.current.leftButton.wasReleasedThisFrame, () => OnLeftButtonRelease?.Invoke()),
        (() => Mouse.current.rightButton.wasPressedThisFrame, () => OnRightButtonPress?.Invoke()),
        (() => Mouse.current.rightButton.wasReleasedThisFrame, () => OnRightButtonRelease?.Invoke()),
        (() => Keyboard.current[Key.Pause].wasPressedThisFrame, () => OnPausePress?.Invoke()),
        (() => Keyboard.current[Key.Equals].wasPressedThisFrame, () => OnEqualsPress?.Invoke()),
        (() => Keyboard.current[Key.LeftArrow].wasPressedThisFrame, () => OnLeftArrowKeyPress?.Invoke()),
        (() => Keyboard.current[Key.RightArrow].wasPressedThisFrame, () => OnRightArrowKeyPress?.Invoke()),
        (() => Keyboard.current[Key.LeftBracket].wasPressedThisFrame, () => OnLeftBracketPress?.Invoke()),
        (() => Keyboard.current[Key.RightBracket].wasPressedThisFrame, () => OnRightBracketPress?.Invoke()),
        (() => Keyboard.current[Key.Backslash].wasPressedThisFrame, () => OnBackslashPress?.Invoke()),
        (() => Keyboard.current[Key.Delete].wasPressedThisFrame, () => OnDelPress?.Invoke()),
        (() => Keyboard.current[Key.UpArrow].wasPressedThisFrame, () => OnUpArrowPress?.Invoke()),
        (() => Keyboard.current[Key.DownArrow].wasPressedThisFrame, () => OnDownArrowPress?.Invoke()),
        (() => Keyboard.current[Key.F4].wasPressedThisFrame, () => OnF4Press?.Invoke()),
        (() => Keyboard.current[Key.F5].wasPressedThisFrame, () => OnF5Press?.Invoke()),
        (() => Keyboard.current[Key.F9].wasPressedThisFrame, () => OnF9Press?.Invoke()),
        (() => IsNotTyping() && Keyboard.current[Key.Z].wasPressedThisFrame, () => OnZPress?.Invoke()),
        (() => IsNotTyping() && Keyboard.current[Key.X].wasPressedThisFrame, () => OnXPress?.Invoke()),
        (() => IsNotTyping() && Keyboard.current[Key.N].wasPressedThisFrame, () => OnNPress?.Invoke()),
        (() => IsNotTyping() && Keyboard.current[Key.Q].wasPressedThisFrame, () => OnQPress?.Invoke()),
        (() => IsNotTyping() && Keyboard.current[Key.E].wasPressedThisFrame, () => OnEPress?.Invoke())
    ];

    internal static event Action<bool>? OnShiftButtonHold;
    internal static event Action<bool>? OnFButtonHold;
    internal static event Action<bool>? OnEButtonHold;
    internal static event Action? OnMiddleButtonPress;
    internal static event Action? OnLeftButtonPress;
    internal static event Action? OnLeftButtonRelease;
    internal static event Action? OnRightButtonPress;
    internal static event Action? OnRightButtonRelease;
    internal static event Action? OnLeftArrowKeyPress;
    internal static event Action? OnRightArrowKeyPress;
    internal static event Action? OnPausePress;
    internal static event Action? OnEqualsPress;
    internal static event Action? OnLeftBracketPress;
    internal static event Action? OnRightBracketPress;
    internal static event Action? OnBackslashPress;
    internal static event Action? OnDelPress;
    internal static event Action? OnUpArrowPress;
    internal static event Action? OnDownArrowPress;
    internal static event Action? OnF4Press;
    internal static event Action? OnF5Press;
    internal static event Action? OnF9Press;
    internal static event Action<bool>? OnRightButtonHold;
    internal static event Action<bool>? OnLeftButtonHold;
    internal static event Action<bool>? OnLeftAltButtonHold;
    internal static event Action? OnZPress;
    internal static event Action? OnXPress;
    internal static event Action? OnNPress;
    internal static event Action? OnQPress;
    internal static event Action? OnEPress;

    private static bool IsNotTyping()
    {
        return Helper.LocalPlayer == null || !Helper.LocalPlayer.isTypingChat;
    }

    private void Update()
    {
        if (IsNotTyping())
        {
            OnFButtonHold?.Invoke(Keyboard.current[Key.F].isPressed);
            OnEButtonHold?.Invoke(Keyboard.current[Key.E].isPressed);
        }

        OnShiftButtonHold?.Invoke(Keyboard.current[Key.LeftShift].isPressed);
        OnLeftButtonHold?.Invoke(Mouse.current.leftButton.isPressed);
        OnRightButtonHold?.Invoke(Mouse.current.rightButton.isPressed);
        OnLeftAltButtonHold?.Invoke(Keyboard.current[Key.LeftAlt].isPressed);

        foreach ((var keyPressed, var eventAction) in InputActions)
        {
            if (!keyPressed()) continue;
            eventAction();
        }
    }
}