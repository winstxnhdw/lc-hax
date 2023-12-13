using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static Vector3 Copy(this Vector3 vector) {
        return new Vector3(
            vector.x,
            vector.y,
            vector.z
        );
    }

    public static GameObject Copy(this Transform transform) {
        GameObject gameObject = new();
        gameObject.transform.position = transform.position.Copy();
        gameObject.transform.eulerAngles = transform.eulerAngles.Copy();

        return gameObject;
    }
}
