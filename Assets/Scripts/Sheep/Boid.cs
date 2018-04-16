using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    private GameObject[] flock;
    private List<GameObject> bodyComponents;
    private Vector3 directionVector;
    private Vector3 sightLine;
    private Rigidbody rb;
    private bool isAvoiding;

    public int separationDistance;
    public int neighbourRadius;
    public bool isDebugging;
    public bool isHerded;
    public float speed;
    public float alignWeight;
    public float sepWeight;
    public float cohesionWeight;
    public float raycastDistance = 5f;
    public float minPlayerDist = 15f;

    void Start()
    {
        flock = FlockManager.allBoids;
        directionVector = Vector3.zero;
        rb = gameObject.GetComponent<Rigidbody>();
        bodyComponents = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.name.Equals("FluffyBody"))
                bodyComponents.Add(child.gameObject);
            else if (child.name.Equals("LittleFluffyBody"))
                bodyComponents.Add(child.gameObject);
        }
    }

    void Update()
    {
        Movement();
    }

    //Main body of vector calculations are here
    public void Movement()
    {
        //Don't calculate vectors if far enough away from player
        if ((Vector3.Distance(transform.position, FlockManager.goalTransform.position) < minPlayerDist) && !isHerded)
        {
            Vector3 cohesion = Cohesion();
            Vector3 separation = Separation();
            Vector3 alignment = Alignment();

            //Combine previously calculated vectors, multiply them by their respective weights then normalize the vector
            directionVector = ((cohesion * cohesionWeight) + (separation * sepWeight) + (alignment * alignWeight));
            directionVector.Normalize();

            //Set boid velocity according to direction vector
            rb.velocity = (new Vector3(directionVector.x, 0, directionVector.z) * speed);

            //Setup raycast properties
            RaycastHit hit;
            float shoulderMultiplier = 0.5f;
            Vector3 leftRay = transform.position - (transform.right * shoulderMultiplier);
            Vector3 rightRay = transform.position + (transform.right * shoulderMultiplier);

            //Left shoulder raycast
            if (Physics.Raycast(leftRay, transform.forward, out hit, raycastDistance))
            {
                if (hit.transform != transform && !hit.collider.gameObject.CompareTag("Safe"))
                {
                    Debug.DrawLine(leftRay, hit.point, Color.red);
                    directionVector += hit.normal * 15.0f;
                }
            }

            //Right shoulder raycast
            else if (Physics.Raycast(rightRay, transform.forward, out hit, raycastDistance))
            {
                if (hit.transform != transform && !hit.collider.gameObject.CompareTag("Safe"))
                {
                    Debug.DrawLine(leftRay, hit.point, Color.red);
                    directionVector += hit.normal * 15.0f;
                }
            }

            //Show both when not "colliding"
            else
            {
                Debug.DrawRay(leftRay, transform.forward * raycastDistance, Color.yellow);
                Debug.DrawRay(rightRay, transform.forward * raycastDistance, Color.yellow);
            }

            //directionVector += separation;

            Quaternion lookRot = Quaternion.LookRotation(directionVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 2.5f * Time.deltaTime);

            rb.velocity = transform.forward * speed;

            //Set boid to show specific vectors
            if (isDebugging)
            {
                Debug.DrawRay(new Vector3(0f, 0f, 0f), cohesion, Color.red); // Cohesion
                Debug.DrawRay(transform.position, separation, Color.blue); // Separation
                Debug.DrawRay(transform.position, alignment, Color.green); // Alignment
            }
        }
        else 
        {
            StartCoroutine(SlowDown());
        }
    }

    // cohesion: steer to move toward the average position (center of mass) of local flockmates
    Vector3 Cohesion()
    {
        //Want percieved rather than actual as boid is not considering itself
        Vector3 percievedCenter = Vector3.zero;
        int count = 0;
        foreach ( GameObject boid in flock )
        {
            if ( boid != gameObject )
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if ( distance > 0 && distance < neighbourRadius )
                {
                    percievedCenter += boid.transform.position;
                    count++;
                }
            }
        }

        if ( count == 0 )
        {
            return percievedCenter + (TendToPlace() - transform.position);
        }
        percievedCenter = percievedCenter / count + (TendToPlace() - transform.position);
        return ( percievedCenter );
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

    //Goal seek but reversed to give herding effect
    Vector3 TendToPlace()
    {
        Vector3 place = FlockManager.goalTransform.position;

        return( place - transform.position ) * -1;
    }

    //Only called when facing player in sheep's FOV
    public void BarkedAt()
    {
        if (Vector3.Dot(transform.forward, (FlockManager.goalTransform.position - transform.position).normalized) > 0.7)
        {
            transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
            StartCoroutine(ColourBriefly());
        }
    }

    //Flash to indicate the sheep was "scared"
    private IEnumerator ColourBriefly()
    {
        foreach (GameObject go in bodyComponents)
            go.GetComponent<MeshRenderer>().material.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        foreach (GameObject go in bodyComponents)
            go.GetComponent<MeshRenderer>().material.color = Color.white;

    }

    //Used when the boid enters the goal area
    public IEnumerator PenEntered()
    {
        yield return new WaitForSeconds(2.0f);
        isHerded = true;
    }

    //Slows boid down when either in pen or no longer chased by player
    public IEnumerator SlowDown()
    {
        Vector3 stopVector = new Vector3(0, 0, 0);
        while (rb.velocity != stopVector)
        {
            rb.velocity -= new Vector3(0.1f ,0 ,0.1f);
            if (rb.velocity.x < 0 || rb.velocity.z < 0)
                rb.velocity = new Vector3(0, 0, 0);
            yield return null;
        }
    }
}

