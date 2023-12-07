using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hax;

public class InputListener : MonoBehaviour {
    Dictionary<Func<bool>, Action> keyActionsDict = new() {
        { () => Keyboard.current[Key.Backquote].wasPressedThisFrame, Console.Open },
    };

    void Update() {
        this.KeyboardListener();
    }

    void KeyboardListener() {
        foreach (KeyValuePair<Func<bool>, Action> keyAction in this.keyActionsDict) {
            if (!keyAction.Key()) continue;
            keyAction.Value();
        }
    }
}
