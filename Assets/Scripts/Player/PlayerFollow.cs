using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour {

    public float MinFollowDist;

	void Update ()
    {
        Vector2 playerDist = new Vector2(FlockManager.goalTransform.position.x - transform.position.x, FlockManager.goalTransform.position.z - transform.position.z);
        if (playerDist.magnitude > MinFollowDist)
        {
            transform.position += new Vector3(playerDist.x / 15.0f, 0, playerDist.y / 15.0f);
        }

        Vector3 newRotation = transform.eulerAngles;
        newRotation.y = FlockManager.goalTransform.eulerAngles.y;
        transform.eulerAngles = newRotation;
        
	}
}
