using System;
using System.Collections.Generic;
using Hax;
using UnityEngine;

internal sealed class ChatMod : MonoBehaviour {
    List<string> CommandHistory { get; } = [];
    int HistoryIndex { get; set; } = -1;

    void OnEnable() {
        InputListener.OnUpArrowPress += this.CycleBackInHistory;
        InputListener.OnDownArrowPress += this.CycleForwardInHistory;
        Chat.OnExecuteCommandAttempt += this.OnHistoryAdded;
    }

    void OnDisable() {
        InputListener.OnUpArrowPress -= this.CycleBackInHistory;
        InputListener.OnDownArrowPress -= this.CycleForwardInHistory;
        Chat.OnExecuteCommandAttempt -= this.OnHistoryAdded;
    }

    void OnHistoryAdded(string command) {
        _ = this.CommandHistory.Remove(command);
        this.CommandHistory.Add(command);
    }

    void CycleBackInHistory() {
        if (Helper.LocalPlayer?.isTypingChat is not true) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;

        this.HistoryIndex = Math.Clamp(this.HistoryIndex + 1, 0, this.CommandHistory.Count - 1);
        int commandHistoryIndex = this.CommandHistory.Count - this.HistoryIndex - 1;
        hudManager.chatTextField.text = this.CommandHistory[commandHistoryIndex];
        hudManager.chatTextField.caretPosition = hudManager.chatTextField.text.Length;
    }

    void CycleForwardInHistory() {
        if (this.HistoryIndex < 0) return;
        if (Helper.LocalPlayer?.isTypingChat is not true) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;

        this.HistoryIndex = Math.Clamp(this.HistoryIndex - 1, 0, this.CommandHistory.Count - 1);
        int commandHistoryIndex = this.CommandHistory.Count - this.HistoryIndex - 1;
        hudManager.chatTextField.text = this.CommandHistory[commandHistoryIndex];
        hudManager.chatTextField.caretPosition = hudManager.chatTextField.text.Length;
    }

    void Update() => this.HistoryIndex = Helper.LocalPlayer?.isTypingChat is not false ? this.HistoryIndex : -1;
}
