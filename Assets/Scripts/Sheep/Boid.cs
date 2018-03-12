using UnityEngine;

public class Boid : MonoBehaviour
{

    private GameObject[] flock;
    private Vector3 directionVector;
    private Vector3 sightLine;
    private Rigidbody rb;
    private const int MAX_SEE_AHEAD = 4;
    private const int MAX_AVOID_FORCE = 100;

    public float speed;
    public int separationDistance;
    public int neighbourRadius;
    public bool isDebugging;
    public float alignWeight;
    public float sepWeight;
    public float cohesionWeight;

    void Start()
    {
        flock = FlockManager.allBoids;
        directionVector = Vector3.zero;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 cohesion = Cohesion();
        Vector3 separation = Separation();
        Vector3 alignment = Alignment();
        Vector3 avoidance = Avoidance();
        sightLine = transform.position + directionVector * MAX_SEE_AHEAD;
        directionVector = ( (cohesion * cohesionWeight) + (separation * sepWeight) + (alignment * alignWeight) + avoidance);
        directionVector.Normalize();
        

        transform.rotation = Quaternion.LookRotation(new Vector3(directionVector.x, 0, directionVector.z));

        rb.velocity = (new Vector3(directionVector.x, 0, directionVector.z) * speed);

        if (isDebugging)
        {
            //Debug.DrawRay(new Vector3(0f, 0f, 0f), cohesion, Color.red); // Cohesion
            //Debug.DrawRay(transform.position, separation, Color.blue); // Separation
            //Debug.DrawRay(transform.position, alignment, Color.green); // Alignment
            Debug.DrawRay(transform.position, sightLine, Color.yellow); // Sightline
            Debug.DrawRay(sightLine, avoidance, Color.cyan); // Avoidance force
        }
    }

    //Determines if boid is about to encounter object, then calculates appropriate avoidance force
    public Vector3 Avoidance()
    {
        Vector3 avoidanceForce =  new Vector3();
        RaycastHit hit;
        RaycastHit[] hits;

        //Get all objects in sightline
        hits = Physics.RaycastAll(transform.position, sightLine, Vector3.Magnitude(sightLine));
        Debug.Log(hits.Length);
        if (hits[0].transform != FlockManager.goalTransform && hits != null)
        {
            if (Physics.Raycast(transform.position, sightLine, out hit, Vector3.Magnitude(sightLine)))
            {
                avoidanceForce = sightLine - hit.point;
                avoidanceForce = Vector3.Normalize(avoidanceForce) * MAX_AVOID_FORCE;
            }
        }

        else
        {
            avoidanceForce *= 0;
        }

        return avoidanceForce;
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

        return(place - gameObject.transform.position);
    }
}

