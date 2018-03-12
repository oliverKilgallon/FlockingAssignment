using UnityEngine;

public class FlockManager : MonoBehaviour
{

    public GameObject boidPrefab;
    public GameObject goal;

    public static Transform goalTransform;
    public static int fieldSize = 10;

    static int numBoids = 7;
    public static GameObject[] allBoids = new GameObject[numBoids];

	// Use this for initialization
	void Start ()
    {
        goalTransform = goal.transform;
        for (int i = 0; i < numBoids; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-fieldSize, fieldSize),
                                      0.5f,
                                      Random.Range(-fieldSize, fieldSize));

            allBoids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
        }
	}

    void Update()
    {
        goalTransform = goal.transform;
    }
}
