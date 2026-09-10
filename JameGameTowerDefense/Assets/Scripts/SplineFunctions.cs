using UnityEngine;
using UnityEngine.Splines;

public static class SplineFunctions
{
    
    public static void SplineEvaluate(SplineContainer spline, float dist, out Vector3 Position, out float t)
    {
        float length = spline.CalculateLength();
        dist = Mathf.Clamp(dist, 0f, length);

        t = spline.Spline.ConvertIndexUnit(dist, PathIndexUnit.Distance, PathIndexUnit.Normalized);
        
        Position= spline.EvaluatePosition(t);
    }
}
