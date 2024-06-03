#region

using System;
using UnityEngine;

#endregion

class ScreenListener : MonoBehaviour {
    int LastScreenWidth { get; set; } = Screen.width;
    int LastScreenHeight { get; set; } = Screen.height;
    internal static event Action? OnScreenSizeChange;

    void Update() => this.ScreenSizeListener();

    void ScreenSizeListener() {
        if (Screen.width == this.LastScreenWidth && Screen.height == this.LastScreenHeight) return;

        this.LastScreenWidth = Screen.width;
        this.LastScreenHeight = Screen.height;
        OnScreenSizeChange?.Invoke();
    }
}
