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
                    boid.GetComponent<Boid>().BarkedAt();
                }
            }
        }
	}
}
