/*
*　　説明　
*　　日付
*　　
*　　原田　智大
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbitScript  : MonoBehaviour
{
    public static List<Vector3> PseudoParabolaPoses(Vector3 position, Vector3 direction, float distance, out float sumDistance,
             float fallCurveVolume = 1f, float fallCurveDelta = 0.1f, float maxFallDistance = 100f, Vector3? fallVector = null)
    {
        sumDistance = 0f;

        // /* If you want to speed it up even a little, comment it out.
        direction.Normalize();
        fallCurveVolume = Mathf.Max(0.01f, fallCurveVolume);
        fallCurveDelta = Mathf.Max(0.01f, fallCurveDelta);
        // */

        if (!fallVector.HasValue) fallVector = Vector3.down;
        var list = new List<Vector3>() { position };

        position = position + direction * distance;
        sumDistance += distance;
        list.Add(position);

        var diffDirectionRate = Mathf.Acos(Vector3.Dot(direction, fallVector.Value));
        if (diffDirectionRate > 0f)
        {
            // Reciprocal of PI : 0.31830988618f
            var fallLength = diffDirectionRate * 0.31830988618f;
            fallCurveDelta /= fallLength;
            var fallStep = 0f;
            while (fallStep < 1f)
            {
                fallStep += fallCurveDelta;
                if (fallStep >= 1f)
                {
                    fallCurveDelta = fallStep - 1f;
                    fallStep = 1f;
                }
                var dir = Vector3.SlerpUnclamped(direction, fallVector.Value, fallStep);
                var addVector = fallCurveDelta * fallCurveVolume;
                position += dir * addVector;
                sumDistance += addVector;
                list.Add(position);
            }
        }
        sumDistance += maxFallDistance;
        list.Add(position + fallVector.Value * maxFallDistance);
        return list;
    }
}


