using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour {

    public float MinFollowDist;

    [Range(1.0f, 100.0f)]
    public float divisor;

	void Update ()
    {
        Vector2 playerDist = new Vector2(FlockManager.goalTransform.position.x - transform.position.x, FlockManager.goalTransform.position.z - transform.position.z);
        if (playerDist.magnitude > MinFollowDist)
        {
            transform.position += new Vector3(playerDist.x / divisor, 0, playerDist.y / divisor);
        }

        Vector3 newRotation = transform.eulerAngles;
        newRotation.y = FlockManager.goalTransform.eulerAngles.y;
        transform.eulerAngles = newRotation;
        
	}
}
