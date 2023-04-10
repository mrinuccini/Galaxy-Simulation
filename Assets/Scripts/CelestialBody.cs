using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [HideInInspector] public Vector3 speed = Vector3.zero;

    /* One simulation step */
    public void Simulate(List<CelestialBody> bodies) 
    {
        Vector3 acceleration = Vector3.zero;

        /* Calculate the acceleration of the bodies */
        foreach(CelestialBody body in bodies) 
        {
            if (body == this) continue;

			// a = direction/d^2 (with the direction being normalized)
			acceleration += (body.transform.position - transform.position).normalized / (Mathf.Pow(Vector3.Distance(transform.position, body.transform.position), 2) + 1);
        }

        speed += acceleration * Time.deltaTime;
        transform.position += speed * Time.deltaTime;
    }
}
