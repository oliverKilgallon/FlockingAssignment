using UnityEngine;

public class FlockManager : MonoBehaviour
{

    public GameObject boidPrefab;
    public GameObject goal;
    public GameObject gate;

    public static Transform goalTransform;
    public static int fieldSize = 10;
    public static int sheepInField;

    public static int numBoids = 7;
    public static GameObject[] allBoids = new GameObject[numBoids];

	// Use this for initialization
	void Start ()
    {
        goalTransform = goal.transform;
        sheepInField = numBoids;
        goalTransform = goal.transform;
        for (int i = 0; i < numBoids; i++)
        {
            Vector3 pos = new Vector3(RandomFloat(Random.Range(-34, -14), Random.Range(4, 34)),
                                      0.5f,
                                      RandomFloat(Random.Range(-20, 10), Random.Range(30, 50)));

            allBoids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
        }
    }

    private float RandomFloat(float lower, float higher)
    {
        float decider = Random.Range(0, 2);
        if (decider == 0)
            return lower;
        else if(decider == 1)
            return higher;

        return lower;
    }

    private void Update()
    {
        if (sheepInField.Equals(0))
        {
            gate.SetActive(true);
            gate.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
