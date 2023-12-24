using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hax;

public class InputListener : MonoBehaviour {
    public static event Action<bool>? onShiftButtonHold;
    public static event Action<bool>? onFButtonHold;
    public static event Action<bool>? onEButtonHold;
    public static event Action<bool>? onRButtonHold;
    public static event Action? onLeftArrowKeyPress;
    public static event Action? onRightArrowKeyPress;
    public static event Action? onEqualsPress;
    public static event Action? onMiddleButtonPress;
    public static event Action? onLeftButtonPress;
    public static event Action? onLeftBracketPress;
    public static event Action? onRightBracketPress;
    public static event Action? onZPress;

    Dictionary<Func<bool>, Action> InputActions { get; } = new() {
        { () => true, () => InputListener.onShiftButtonHold?.Invoke(Keyboard.current[Key.LeftShift].isPressed) },
        { () => true, () => InputListener.onFButtonHold?.Invoke(Keyboard.current[Key.F].isPressed) },
        { () => true, () => InputListener.onRButtonHold?.Invoke(Keyboard.current[Key.R].isPressed) },
        { () => true, () => InputListener.onEButtonHold?.Invoke(Keyboard.current[Key.E].isPressed) },
        { () => Keyboard.current[Key.Equals].wasPressedThisFrame, () => InputListener.onEqualsPress?.Invoke() },
        { () => Keyboard.current[Key.LeftArrow].wasPressedThisFrame, () => InputListener.onLeftArrowKeyPress?.Invoke() },
        { () => Keyboard.current[Key.RightArrow].wasPressedThisFrame, () => InputListener.onRightArrowKeyPress?.Invoke() },
        { () => Keyboard.current[Key.LeftBracket].wasPressedThisFrame, () => InputListener.onLeftBracketPress?.Invoke() },
        { () => Keyboard.current[Key.RightBracket].wasPressedThisFrame, () => InputListener.onRightBracketPress?.Invoke() },
        { () => Mouse.current.middleButton.wasPressedThisFrame, () => InputListener.onMiddleButtonPress?.Invoke() },
        { () => Mouse.current.leftButton.wasPressedThisFrame, () => InputListener.onLeftButtonPress?.Invoke() },
        { () => Keyboard.current[Key.Z].wasPressedThisFrame, () => InputListener.onZPress?.Invoke() },
    };

    void Update() {
        foreach (KeyValuePair<Func<bool>, Action> keyAction in this.InputActions) {
            if (Helper.LocalPlayer is not null && Helper.LocalPlayer.isTypingChat) break;
            if (!keyAction.Key()) continue;
            keyAction.Value();
        }
    }
}
