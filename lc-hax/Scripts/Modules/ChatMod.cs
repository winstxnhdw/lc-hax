using System;
using System.Collections.Generic;
using Hax;
using UnityEngine;

internal sealed class ChatMod : MonoBehaviour
{
    private List<string> CommandHistory { get; } = [];
    private int HistoryIndex { get; set; } = -1;

    private void OnEnable()
    {
        InputListener.OnUpArrowPress += CycleBackInHistory;
        InputListener.OnDownArrowPress += CycleForwardInHistory;
        Chat.OnExecuteCommandAttempt += OnHistoryAdded;
    }

    private void OnDisable()
    {
        InputListener.OnUpArrowPress -= CycleBackInHistory;
        InputListener.OnDownArrowPress -= CycleForwardInHistory;
        Chat.OnExecuteCommandAttempt -= OnHistoryAdded;
    }

    private void OnHistoryAdded(string command)
    {
        _ = CommandHistory.Remove(command);
        CommandHistory.Add(command);
    }

    private void CycleBackInHistory()
    {
        if (Helper.LocalPlayer is not { isTypingChat: true }) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;

        HistoryIndex = Math.Clamp(HistoryIndex + 1, 0, CommandHistory.Count - 1);
        var commandHistoryIndex = CommandHistory.Count - HistoryIndex - 1;
        hudManager.chatTextField.text = CommandHistory[commandHistoryIndex];
        hudManager.chatTextField.caretPosition = hudManager.chatTextField.text.Length;
    }

    private void CycleForwardInHistory()
    {
        if (HistoryIndex < 0) return;
        if (Helper.LocalPlayer is not { isTypingChat: true }) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;

        HistoryIndex = Math.Clamp(HistoryIndex - 1, 0, CommandHistory.Count - 1);
        var commandHistoryIndex = CommandHistory.Count - HistoryIndex - 1;
        hudManager.chatTextField.text = CommandHistory[commandHistoryIndex];
        hudManager.chatTextField.caretPosition = hudManager.chatTextField.text.Length;
    }

    private void Update()
    {
        HistoryIndex = Helper.LocalPlayer is { isTypingChat: true } ? HistoryIndex : -1;
    }
}