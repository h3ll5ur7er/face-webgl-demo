
using UnityEngine;

public static class VectorExtensions {

    public static RaycastHit? RayCast(this Vector2 screenPos, string layer = null, int maxDistance=10000) {
        if(Physics.Raycast(Camera.main.ScreenPointToRay(screenPos), out var hit, 10000, LayerMask.GetMask(layer))) {
            return hit;
        }
        return null;
    }
    public static RaycastHit? RayCast(this Vector3 screenPos, string layer, int maxDistance=10000) {
        return RayCast((Vector2)screenPos, layer, maxDistance);
    }
    public static Vector3 Div(this Vector3 v1, Vector3 v2) {
        return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }
    public static Vector3 Mul(this Vector3 v1, Vector3 v2) {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }
}
