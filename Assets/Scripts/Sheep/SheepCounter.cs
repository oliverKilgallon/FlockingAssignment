﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepCounter : MonoBehaviour {
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            FlockManager.sheepInField++;
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            FlockManager.sheepInField--;
            //StartCoroutine(other.GetComponent<Boid>().PenEntered());
        }
    }
}
