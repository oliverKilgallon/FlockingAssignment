using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    private GameObject[] flock;
    private Vector3 directionVector;

    public float speed;
    public int separationDistance;
    public int neighbourRadius;
    public bool isDebugging;

    void Start()
    {
        flock = GameObject.FindGameObjectsWithTag("Boid");
        directionVector = Vector3.zero;
    }
    void Update()
    {
        Vector3 v1 = Cohesion();
        Vector3 v2 = Separation();
        Vector3 v3 = Alignment();

        if (isDebugging)
        {
            Debug.DrawRay(transform.position, v1, Color.red);
            Debug.DrawRay(transform.position, v2, Color.blue);
            Debug.DrawRay(transform.position, v3, Color.green);
        }

        directionVector = v1 + v2 + v3;

        //transform.rotation = Quaternion.LookRotation(velocity.normalized);

        directionVector.Normalize();
        
        transform.position += new Vector3(directionVector.x, 0, directionVector.y) * speed * Time.deltaTime;
    }

    // cohesion: steer to move toward the average position (center of mass) of local flockmates
    Vector3 Cohesion()
    {
        //Want percieved rather than actual as boid is not considering itself
        Vector3 percievedCenter = Vector3.zero;

        foreach (GameObject boid in flock)
        {
            if (boid != gameObject)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if(distance > 0 && distance < neighbourRadius)
                    percievedCenter += boid.transform.position;
            }
        }

        //Averaging positions while taking into acc the boid calling this
        percievedCenter = percievedCenter / (flock.Length);

        //Dividing by 100 gives the amount to move by 1% in the given direction
        return (percievedCenter - gameObject.transform.position);
    }

    // separation: steer to avoid crowding local flockmates
    Vector3 Separation()
    {
        Vector3 resultVector = Vector3.zero;
        int count = 0;
        foreach (GameObject boid in flock)
        {
                //if (Vector3.Magnitude(boid.transform.position - gameObject.transform.position) < separationMinimumDistance)
                //{
                //    resultVector -= (boid.transform.position - gameObject.transform.position);
                //}
                float distance = Vector3.Distance(gameObject.transform.position, boid.transform.position);
                if (distance > 0 && distance < separationDistance)
                    resultVector += (gameObject.transform.position - boid.transform.position);
                count++;
        }
        //if (count > 0)
        //    resultVector /= count;
        
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
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance > 0 && distance < neighbourRadius)
                    percievedVelocity += boid.transform.TransformDirection(boid.transform.forward);
            }
        }

        //Averaging velocities while taking into acc the boid calling this
        percievedVelocity /= (flock.Length - 1);

        //Adding only an eighth to boid's velocity
        return (percievedVelocity - gameObject.transform.position);
    }

    //void OnTriggerEnter(Collider otherCollider)
    //{
    //    if (otherCollider.tag == "Boid")
    //        flock.Add(otherCollider.gameObject);
    //}

    //void OnTriggerExit(Collider otherCollider)
    //{
    //    if(otherCollider.tag == "Boid")
    //        flock.Remove(otherCollider.gameObject);
    //}
}

