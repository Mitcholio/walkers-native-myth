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

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode flashlightKey = KeyCode.F;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode inventoryKey = KeyCode.Tab;
    public KeyCode keepItemKey = KeyCode.R;
    public KeyCode helpKey = KeyCode.F1;

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
    }

    public float movementSqrMag()
    {
        return moveMg = new Vector2(moveH, moveV).sqrMagnitude;
    }

    public float viewSqrMag()
    {
        return viewMg = new Vector2(viewH, viewV).sqrMagnitude;
    }

    public bool Jump()
    {
        return Input.GetKeyDown(jumpKey);
    }

    public bool Sprint()
    {
        return Input.GetKey(sprintKey);
    }

    public bool Crouch()
    {
        return Input.GetKeyDown(crouchKey);
    }

    public bool Flashlight()
    {
        return Input.GetKeyDown(flashlightKey);
    }

    public bool Pickup()
    {
        return Input.GetKeyDown(pickupKey);
    }

    public bool Inventory()
    {
        return Input.GetKeyDown(inventoryKey);
    }

    public bool KeepItem()
    {
        return Input.GetKeyDown(keepItemKey);
    }

    public bool Help()
    {
        return Input.GetKeyDown(helpKey);
    }
}
