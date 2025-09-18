using UnityEngine;

public static class ForceCalculator 
{
    public static Vector3 CalculateForce(Vector2 mousePressed, Vector2 mouseReleased, float maxForce)
    {
        Vector3 force = mousePressed - mouseReleased;

        if (force.magnitude > maxForce)
            force = force.normalized * maxForce;

        return force;
    }

    public static float Map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = OldMax - OldMin;
        float NewRange = NewMax - NewMin;
        float NewValue = ((OldValue - OldMin) * NewRange / OldRange) + NewMin;

        return NewValue;
    }
}
