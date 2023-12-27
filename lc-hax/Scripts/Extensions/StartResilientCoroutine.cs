using System.Collections;
using UnityEngine;

namespace Hax;

public static partial class Extensions {
    public static Coroutine StartResilientCoroutine(this MonoBehaviour self, IEnumerator coroutine) {
        static IEnumerator ResilientCoroutine(IEnumerator coroutine) {
            bool isSafe;

            while (true) {
                try {
                    if (!coroutine.MoveNext()) yield break;
                    isSafe = true;
                }

                catch {
                    isSafe = false;
                }

                yield return isSafe ? coroutine.Current : new WaitForSeconds(1.0f);
            }
        }

        return self.StartCoroutine(ResilientCoroutine(coroutine));
    }
}
