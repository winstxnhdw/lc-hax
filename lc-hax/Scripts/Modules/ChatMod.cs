using System.Collections;
using UnityEngine;

namespace Hax;

public class ChatMod : MonoBehaviour {
    IEnumerator ShowHUD() {
        while (true) {
            Helpers.HUDManager?.HideHUD(false);
            yield return new WaitForSeconds(1.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.ShowHUD());
    }
}
