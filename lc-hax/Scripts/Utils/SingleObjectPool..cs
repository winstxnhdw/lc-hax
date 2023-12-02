using System.Collections;
using UnityObject = UnityEngine.Object;
using UnityEngine;

namespace Hax;

public class SingleObjectPool<T> where T : UnityObject {
    public T? Object { get; private set; }

    public SingleObjectPool(MonoBehaviour self, float renewInterval = 5.0f) {
        _ = self.StartCoroutine(this.RenewObject(renewInterval));
    }

    IEnumerator RenewObject(float renewInterval) {
        while (true) {
            this.Object = UnityObject.FindObjectOfType<T>();
            yield return new WaitForSeconds(renewInterval);
        }
    }
}
