using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class NamesMod : MonoBehaviour {
    IEnumerator ShowNames() {
        while (true) {
            Helper.Players?.ForEach(player => {
                player.ShowNameBillboard();
            });

            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.ShowNames());
    }
}
