using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    private List<GameObject> flock = new List<GameObject>();
    private Vector3 directionVector;

    public float speed = 1.0f;
    public int separationMinimumDistance;

    void Update()
    {
        foreach (GameObject obj in flock)
        {
            if (obj == null)
                flock.Remove(obj);
        }

        directionVector = Separation() + Alignment() + Cohesion();

        //transform.rotation = Quaternion.LookRotation(velocity.normalized);

        transform.position += new Vector3(directionVector.normalized.x, 0, directionVector.normalized.y) * speed * Time.deltaTime;
    }

    // cohesion: steer to move toward the average position (center of mass) of local flockmates
    Vector3 Cohesion()
    {
        //Want percieved rather than actual as boid is not considering itself
        Vector3 percievedCenter = Vector3.zero;

        foreach (GameObject boid in flock)
        {
            if (boid != this)
            {
                percievedCenter += boid.transform.position;
            }
        }

        //Averaging positions while taking into acc the boid calling this
        percievedCenter /= (flock.Count - 1);

        //Dividing by 100 gives the amount to move by 1% in the given direction
        return (percievedCenter - gameObject.transform.position) / 100;
    }

    // separation: steer to avoid crowding local flockmates
    Vector3 Separation()
    {
        Vector3 resultVector = Vector3.zero;
        foreach (GameObject boid in flock)
        {
            if (boid != gameObject)
            {
                if (Vector3.Magnitude(boid.transform.position - gameObject.transform.position) < separationMinimumDistance)
                {
                    resultVector -= (boid.transform.position - gameObject.transform.position);
                }
            }
        }
        return resultVector;
    }

    // alignment: steer towards the average heading of local flockmates
    Vector3 Alignment()
    {
        //Want percieved rather than actual as boid is not considering itself
        Vector3 percievedVelocity = Vector3.zero;

        foreach (GameObject boid in flock)
        {
            if (boid != this)
            {
                percievedVelocity += boid.transform.TransformDirection(boid.transform.forward);
            }
        }

        //Averaging velocities while taking into acc the boid calling this
        percievedVelocity /= (flock.Count - 1);

        //Adding only an eighth to boid's velocity
        return (percievedVelocity - gameObject.transform.position) / 8;
    }
    
    void OnTriggerEnter(Collider otherCollider)
    {
        if(otherCollider.tag == "Boid")
            flock.Add(otherCollider.gameObject);
    }

    void OnTriggerExit(Collider otherCollider)
    {
        flock.Remove(otherCollider.gameObject);
    }
}

