using System;
using UnityEngine;

class ScreenListener : MonoBehaviour {
    internal static event Action? OnScreenSizeChange;

    int LastScreenWidth { get; set; } = Screen.width;
    int LastScreenHeight { get; set; } = Screen.height;

    void Update() => this.ScreenSizeListener();

    void ScreenSizeListener() {
        if (Screen.width == this.LastScreenWidth && Screen.height == this.LastScreenHeight) return;

        this.LastScreenWidth = Screen.width;
        this.LastScreenHeight = Screen.height;
        ScreenListener.OnScreenSizeChange?.Invoke();
    }
}
