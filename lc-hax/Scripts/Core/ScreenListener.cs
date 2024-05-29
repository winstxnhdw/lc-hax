using System;
using UnityEngine;

internal class ScreenListener : MonoBehaviour
{
    private int LastScreenWidth { get; set; } = Screen.width;
    private int LastScreenHeight { get; set; } = Screen.height;
    internal static event Action? OnScreenSizeChange;

    private void Update()
    {
        ScreenSizeListener();
    }

    private void ScreenSizeListener()
    {
        if (Screen.width == LastScreenWidth && Screen.height == LastScreenHeight) return;

        LastScreenWidth = Screen.width;
        LastScreenHeight = Screen.height;
        OnScreenSizeChange?.Invoke();
    }
}