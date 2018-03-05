using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    private GameObject[] flock;
    private Vector3 directionVector;
    private Rigidbody rb;

    public float speed;
    public int separationDistance;
    public int neighbourRadius;
    public bool isDebugging;
    public float alignWeight;
    public float sepWeight;
    public float cohesionWeight;
    private GameObject objective;


    void Start()
    {
        flock = FlockManager.allBoids;
        directionVector = Vector3.zero;
        rb = gameObject.GetComponent<Rigidbody>();
        objective = GameObject.FindGameObjectWithTag("Objective");
    }
    void Update()
    {
        Vector3 cohesion = Cohesion();
        Vector3 separation = Separation();
        Vector3 alignment = Alignment();
        Vector3 tending = TendToPlace();

        if (isDebugging)
        {
            Debug.DrawRay(new Vector3(0f, 0f, 0f), cohesion, Color.red);
            Debug.DrawRay(transform.position, separation, Color.blue);
            Debug.DrawRay(transform.position, alignment, Color.green);
        }

        directionVector = ( (cohesion * cohesionWeight) + (separation * sepWeight) + (alignment * alignWeight) );
        directionVector.Normalize();

        transform.rotation = Quaternion.LookRotation(new Vector3(directionVector.x, 0, directionVector.z));

        rb.velocity = (new Vector3(directionVector.x, 0, directionVector.z) * speed);
    }

    // cohesion: steer to move toward the average position (center of mass) of local flockmates
    Vector3 Cohesion()
    {
        //Want percieved rather than actual as boid is not considering itself
        Vector3 percievedCenter = Vector3.zero;
        int count = 0;
        foreach (GameObject boid in flock)
        {
            if (boid != gameObject)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance > 0 && distance < neighbourRadius)
                {
                    percievedCenter += boid.transform.position;
                    count++;
                    
                }
            }
        }

        if (count == 0)
        {
            return percievedCenter + (TendToPlace() - transform.position);
        }
        percievedCenter = percievedCenter / count + (TendToPlace() - transform.position);
        return (percievedCenter);
    }

    // separation: steer to avoid crowding local flockmates
    Vector3 Separation()
    {
        Vector3 resultVector = Vector3.zero;
        int count = 0;
        foreach (GameObject boid in flock)
        {
            if (boid != gameObject)
            {
                float distance = Vector3.Distance(gameObject.transform.position, boid.transform.position);
                if (distance > 0 && distance < separationDistance)
                {
                    resultVector += (boid.transform.position - gameObject.transform.position);
                    count++;
                }
            }
                
        }

        if (count == 0)
        {
            return resultVector;
        }
        
        return resultVector *= -1;
    }

    // alignment: steer towards the average heading of local flockmates
    Vector3 Alignment()
    {
        //Want percieved rather than actual as boid is not considering itself
        Vector3 percievedVelocity = Vector3.zero;
        int count = 0;
        foreach (GameObject boid in flock)
        {
            if (boid != gameObject)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance > 0 && distance < neighbourRadius)
                {
                    //percievedVelocity += boid.transform.TransformDirection(boid.transform.forward);
                    percievedVelocity += boid.GetComponent<Boid>().rb.velocity;
                    count++;
                }
            }
        }
        if (count == 0)
        {
            return percievedVelocity;
        }
        //Averaging velocities while taking into acc the boid calling this
        percievedVelocity = percievedVelocity / count;
        
        return percievedVelocity;
    }

    Vector3 TendToPlace()
    {
        Vector3 place = objective.transform.position;

        return(place - gameObject.transform.position);
    }
}

