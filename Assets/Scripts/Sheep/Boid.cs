using UnityEngine;

public class Boid : MonoBehaviour
{

    private GameObject[] flock;
    private Vector3 directionVector;
    private Vector3 sightLine;
    private Rigidbody rb;
    private bool isAvoiding;

    public int separationDistance;
    public int neighbourRadius;
    public bool isDebugging;
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
    }

    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        //Don't calculate vectors if close to objective
        if ((Vector3.Distance(transform.position, FlockManager.goalTransform.position) < minPlayerDist))
        {
            Vector3 cohesion = Cohesion();
            Vector3 separation = Separation();
            Vector3 alignment = Alignment();

            directionVector = ((cohesion * cohesionWeight) + (separation * sepWeight) + (alignment * alignWeight));
            directionVector.Normalize();

            //Set boid velocity according to direction vector
            rb.velocity = (new Vector3(directionVector.x, 0, directionVector.z) * speed);

            RaycastHit hit;
            float shoulderMultiplier = 0.5f;

            Vector3 leftRay = transform.position - (transform.right * shoulderMultiplier);
            Vector3 rightRay = transform.position + (transform.right * shoulderMultiplier);

            if (Physics.Raycast(leftRay, transform.forward, out hit, raycastDistance))
            {
                if (hit.transform != transform)
                {
                    Debug.DrawLine(leftRay, hit.point, Color.red);
                    directionVector += hit.normal * 15.0f;
                }
            }

            else if (Physics.Raycast(rightRay, transform.forward, out hit, raycastDistance))
            {
                if (hit.transform != transform)
                {
                    Debug.DrawLine(leftRay, hit.point, Color.red);
                    directionVector += hit.normal * 15.0f;
                }
            }
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


        
    }

    //Determines if boid is about to encounter object, then calculates appropriate avoidance force
    public bool Avoidance()
    {
        RaycastHit hit;

        float shoulderMultiplier = 0.5f;

        Vector3 leftRay = transform.position - (transform.right * shoulderMultiplier);
        Vector3 rightRay = transform.position + (transform.right * shoulderMultiplier);

        bool leftRayHit = Physics.Raycast(leftRay, transform.forward, out hit, raycastDistance);
        bool rightRayHit = Physics.Raycast(rightRay, transform.forward, out hit, raycastDistance);
        bool centre = Physics.Raycast(transform.position, transform.forward, raycastDistance);

        return (leftRayHit || centre) || rightRayHit;
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
        Vector3 place = FlockManager.goalTransform.position;

        return(place - gameObject.transform.position) * -1;
    }
}

