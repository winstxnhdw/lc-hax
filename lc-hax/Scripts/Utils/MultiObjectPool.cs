using System.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

public class MultiObjectPool<T> where T : UnityObject {
    public T?[] Objects { get; private set; } = [];

    public MultiObjectPool(MonoBehaviour self, float renewInterval = 1.0f) {
        _ = self.StartCoroutine(this.RenewObjects(renewInterval));
    }

    public void Renew() {
        this.Objects = UnityObject.FindObjectsByType<T>(FindObjectsSortMode.None);
    }

    IEnumerator RenewObjects(float renewInterval) {
        while (true) {
            this.Renew();
            yield return new WaitForSeconds(renewInterval);
        }
    }
}
