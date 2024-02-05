using System.Collections;
using UnityObject = UnityEngine.Object;
using UnityEngine;

public class SingleObjectPool<T> where T : UnityObject {
    public T? Object { get; private set; }

    public SingleObjectPool(MonoBehaviour self, float renewInterval = 1.0f) => _ = self.StartCoroutine(this.RenewObject(renewInterval));

    public void Renew() => this.Object = UnityObject.FindAnyObjectByType<T>();

    IEnumerator RenewObject(float renewInterval) {
        WaitForSeconds waitForRenewInterval = new(renewInterval);

        while (true) {
            this.Renew();
            yield return waitForRenewInterval;
        }
    }
}
