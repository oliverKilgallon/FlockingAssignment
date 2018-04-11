using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepCounter : MonoBehaviour {

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            FlockManager.sheepInField++;
            Debug.Log(FlockManager.sheepInField + " Sheep");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            FlockManager.sheepInField--;
        }
    }
}
