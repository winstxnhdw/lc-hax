using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hax;
public class InputListener : MonoBehaviour {
    public static event Action? onEqualsPress;
    public static event Action? onMiddleButtonPress;
    public static event Action? onLeftButtonPress;

    Dictionary<Func<bool>, Action> InputActions { get; } = new() {
        { () => Keyboard.current[Key.Equals].wasPressedThisFrame, () => InputListener.onEqualsPress?.Invoke() },
        { () => Mouse.current.middleButton.wasPressedThisFrame, () => InputListener.onMiddleButtonPress?.Invoke() },
        { () => Mouse.current.leftButton.wasPressedThisFrame, () => InputListener.onLeftButtonPress?.Invoke() }
    };

    void Update() {
        foreach (KeyValuePair<Func<bool>, Action> keyAction in this.InputActions) {
            if (!keyAction.Key()) continue;
            keyAction.Value();
        }
    }
}
