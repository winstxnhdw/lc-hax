using System.Collections;
using Hax;
using UnityEngine;

internal sealed class AntiKickMod : MonoBehaviour
{
    private void OnEnable()
    {
        InputListener.OnBackslashPress += ToggleAntiKick;
        GameListener.OnGameStart += OnGameStart;
        GameListener.OnGameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        InputListener.OnBackslashPress -= ToggleAntiKick;
        GameListener.OnGameStart -= OnGameStart;
        GameListener.OnGameEnd -= OnGameEnd;
    }

    private IEnumerator RejoinLobby()
    {
        if (State.ConnectedLobby is not ConnectedLobby connectedLobby) yield break;

        WaitForEndOfFrame waitForEndOfFrame = new();

        while (Helper.FindObject<MenuManager>() is null) yield return waitForEndOfFrame;

        while (Helper.GameNetworkManager?.currentLobby.HasValue is not false) yield return waitForEndOfFrame;

        Helper.GameNetworkManager?.JoinLobby(connectedLobby.Lobby, connectedLobby.SteamId);
    }

    private void OnGameEnd()
    {
        if (State.DisconnectedVoluntarily || !Setting.EnableAntiKick) return;
        _ = StartCoroutine(RejoinLobby());
    }

    private void OnGameStart()
    {
        if (!Setting.EnableAntiKick || !Setting.EnableInvisible) return;

        Chat.Clear();
        Helper.SendNotification(
            "Anti-kick",
            "You are invisible to other players! Do /invis to disable!",
            true
        );
    }

    private void ToggleAntiKick()
    {
        if (Helper.LocalPlayer is not null)
        {
            Chat.Print("You cannot toggle anti-kick while in-game!");
            return;
        }

        Setting.EnableAntiKick = !Setting.EnableAntiKick;
        Setting.EnableInvisible = Setting.EnableAntiKick;
    }
}