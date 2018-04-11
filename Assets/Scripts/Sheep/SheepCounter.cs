using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepCounter : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            FlockManager.sheepInField++;
            Debug.Log(FlockManager.sheepInField + " Sheep");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            FlockManager.sheepInField--;
        }
    }
}
