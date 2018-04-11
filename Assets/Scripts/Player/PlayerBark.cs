using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBark : MonoBehaviour {
    
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Jump"))
        {
            foreach (GameObject boid in FlockManager.allBoids)
            {
                if (Vector3.Distance(gameObject.transform.position, boid.transform.position) < 10.0f)
                {
                    if (Vector3.Angle(transform.forward, boid.transform.position - transform.position) > 30)
                        boid.GetComponent<Boid>().BarkedAt();
                }
            }
        }
	}

    //float TargetLocation(GameObject goOne, GameObject goTwo)
    //{
    //    Vector3 heading = goTwo.transform.position - goOne.transform.position;
    //    float dot = Vector3.Dot(heading, goOne.transform.forward);
    //    return dot;
    //}
}
