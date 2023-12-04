using System.Collections;
using UnityObject = UnityEngine.Object;
using UnityEngine;

namespace Hax;

public class SingleObjectPool<T> where T : UnityObject {
    public T? Object { get; private set; }

    public SingleObjectPool(MonoBehaviour self, float renewInterval = 1.0f) {
        _ = self.StartCoroutine(this.RenewObject(renewInterval));
    }

    public void Renew() {
        this.Object = UnityObject.FindObjectOfType<T>();
    }

    IEnumerator RenewObject(float renewInterval) {
        while (true) {
            this.Renew();
            yield return new WaitForSeconds(renewInterval);
        }
    }
}
