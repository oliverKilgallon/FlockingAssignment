using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float m_MoveSpeed;
    public float m_RotSpeed;
	
	void Update ()
    {
        MovementUpdate();	
	}

    void MovementUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        transform.position += transform.forward * moveVertical * m_MoveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, moveHorizontal * m_RotSpeed);
    }
    
}
