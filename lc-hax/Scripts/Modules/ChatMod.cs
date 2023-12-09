using System.Collections;
using UnityEngine;

namespace Hax;

public class ChatMod : MonoBehaviour {
    IEnumerator ShowHUD() {
        while (true) {
            Helper.HUDManager?.HideHUD(false);
            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.ShowHUD());
    }
}
