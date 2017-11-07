using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentList : MonoBehaviour {

    public InputManager IM;
    public PlayerController PC;
    public PlayerActions PA;
    public Inventory Inv;
    public CameraController CamC;
    public Timer timer;
    public PlayerUI PUI;

	// Use this for initialization
	void Start ()
    {
        StartComponents();
	}

    void StartComponents()
    {
        PUI.gameObject.SetActive(true);
    }

}
