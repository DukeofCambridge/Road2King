using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private const float threshold=0.5f;
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vector2target = target.position - transform.position;
        vector2target.Normalize();

        float dot = Vector3.Dot(transform.forward, vector2target);
        return dot >= threshold;
    }
}
