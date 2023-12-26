using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class NamesMod : MonoBehaviour {
    void ShowNameBillboard() => Helper.Players?.ForEach(player => player.ShowNameBillboard());

    IEnumerator ShowNames() {
        while (true) {
            this.ShowNameBillboard();
            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.ShowNames());
    }
}
