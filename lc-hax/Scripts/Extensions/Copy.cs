using UnityEngine;

static partial class Extensions {
    internal static Vector3 Copy(this Vector3 vector) {
        return new Vector3(
            vector.x,
            vector.y,
            vector.z
        );
    }

    internal static Quaternion Copy(this Quaternion quaternion) {
        return new Quaternion(
            quaternion.x,
            quaternion.y,
            quaternion.z,
            quaternion.w
        );
    }

    internal static Transform Copy(this Transform transform) {
        GameObject gameObject = new();
        gameObject.transform.position = transform.position.Copy();
        gameObject.transform.eulerAngles = transform.eulerAngles.Copy();
        gameObject.transform.rotation = transform.rotation.Copy();
        gameObject.transform.localScale = transform.localScale.Copy();

        return gameObject.transform;
    }
}
