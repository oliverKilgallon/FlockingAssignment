using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SheepCounter : MonoBehaviour {

    public Text AmountToCatch;
    public Text NumCaught;

	// Use this for initialization
	void Start ()
    {
        AmountToCatch.text = "/" + FlockManager.numBoids.ToString();
	}
	
	// Update is called once per frame
	void Update ()
    {
        NumCaught.text = FlockManager.sheepInField.ToString();
	}
}
