using System;
using UnityEngine;

namespace Hax;

public class ScreenListener : MonoBehaviour {
    public static event Action? onScreenSizeChange;

    int LastScreenWidth { get; set; } = Screen.width;
    int LastScreenHeight { get; set; } = Screen.height;

    void Update() {
        this.ScreenSizeListener();
    }

    void ScreenSizeListener() {
        if (Screen.width == this.LastScreenWidth && Screen.height == this.LastScreenHeight) return;

        this.LastScreenWidth = Screen.width;
        this.LastScreenHeight = Screen.height;
        ScreenListener.onScreenSizeChange?.Invoke();
    }
}
