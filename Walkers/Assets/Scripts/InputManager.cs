using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public float moveH;
    public float moveV;
    float moveMg;
    public float viewH;
    public float viewV;
    float viewMg;
    public bool jump = false;
    public bool sprint = false;
    public bool crouch = false;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        viewH = Input.GetAxis("Mouse X");
        viewV = Input.GetAxis("Mouse Y");
        jump = Input.GetKeyDown(KeyCode.Space);
        sprint = Input.GetKey(KeyCode.LeftShift);
        crouch = Input.GetKeyDown(KeyCode.LeftControl);
    }

    public float movementSqrMag()
    {
        return moveMg = new Vector2(moveH, moveV).sqrMagnitude;
    }

    public float viewSqrMag()
    {
        return viewMg = new Vector2(viewH, viewV).sqrMagnitude;
    }
}
