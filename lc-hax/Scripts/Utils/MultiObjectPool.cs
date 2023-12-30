using System.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Hax;

public class MultiObjectPool<T> where T : UnityObject {
    public T?[] Objects { get; private set; } = [];

    public MultiObjectPool(MonoBehaviour self, float renewInterval = 1.0f) {
        _ = self.StartCoroutine(this.RenewObjects(renewInterval));
    }

    public void Renew() {
        this.Objects = UnityObject.FindObjectsOfType<T>();
    }

    IEnumerator RenewObjects(float renewInterval) {
        while (true) {
            this.Renew();
            yield return new WaitForSeconds(renewInterval);
        }
    }
}
