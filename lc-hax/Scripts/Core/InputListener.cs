using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public class InputListener : MonoBehaviour {

    Dictionary<Func<bool>, Action> keyActionsDict = new() {
        {() => Input.GetKeyDown(KeyCode.BackQuote),   () => Console.Open()},
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
