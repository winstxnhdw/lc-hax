using System.Collections;
using UnityObject = UnityEngine.Object;
using UnityEngine;

namespace Hax;

public class MultiObjectPool<T> where T : UnityObject {
    public T[]? Objects { get; private set; }

    public MultiObjectPool(MonoBehaviour self, float renewInterval = 5.0f) {
        _ = self.StartCoroutine(this.RenewObjects(renewInterval));
    }

    IEnumerator RenewObjects(float renewInterval) {
        while (true) {
            this.Objects = UnityObject.FindObjectsOfType<T>();
            yield return new WaitForSeconds(renewInterval);
        }
    }
}
