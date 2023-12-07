using UnityEngine;

namespace Hax;

public static partial class Helpers {
    public static Vector3 Copy(Vector3 vector) {
        return new Vector3(
            vector.x,
            vector.y,
            vector.z
        );
    }

    public static GameObject Copy(Transform transform) {
        GameObject gameObject = new();
        gameObject.transform.position = Helpers.Copy(transform.position);
        gameObject.transform.eulerAngles = Helpers.Copy(transform.eulerAngles);

        return gameObject;
    }
}
