using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    InputManager IM;
    public PlayerController Player;
    public Camera cam;
    public float fieldOfView = 90;
    public float SensX = 10;
    public float SensY = 10;
    float xRot = 0;
    float yRot = 0;

    float h = 0;
    float v = 0;

    void Start ()
    {
        StartVars();
        SetFov();
    }

    void StartVars()
    {
        cam = GetComponentInChildren<Camera>();
        IM = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<InputManager>();
    }

    void Update()
    {
        Inputs();
    }

	void FixedUpdate ()
    {
        FollowPlayer();

        Rotate();
        RotatePlayer();
	}

    void Inputs()
    {
        h = IM.viewH;
        v = -IM.viewV;
    }

    void FollowPlayer()
    {
        transform.position = Player.EyePoint.position;
    }

    void RotatePlayer()
    {
        Player.transform.localEulerAngles = new Vector3(0, xRot, 0);
    }

    void Rotate()
    {
        xRot += h * SensX * Time.smoothDeltaTime;
        xRot = xRot % 360;

        yRot += v * SensY * Time.smoothDeltaTime;
        yRot = Mathf.Clamp(yRot, -80, 80);

        transform.localEulerAngles = new Vector3(yRot, xRot, 0);
    }

    void SetFov()
    {
        float hFOVrad = fieldOfView * Mathf.Deg2Rad;
        float camH = Mathf.Tan(hFOVrad * 0.5f) / cam.aspect;
        float vFOVrad = Mathf.Atan(camH) * 2;
        cam.fieldOfView = vFOVrad * Mathf.Rad2Deg;
    }
}
