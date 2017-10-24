using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    ComponentList CL;
    public PlayerController PC;
    public PlayerActions PA;
    Inventory Inv;
    public Text HoverInfoText;
    public Text curItemInfoText;
    public Text ControlsInfoText;
    Camera cam;

    bool InvOpen = false;


    // Use this for initialization
    void Start ()
    {
        StartVars();
	}

    void StartVars()
    {
        CL = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<ComponentList>();
        Inv = PA.Inv;
        cam = Camera.main;
        ControlsInfoText.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        HoverItemPopup();
        EquipInfo();
	}

    void Update()
    {
        ControlsInfo();
    }

    void HoverItemPopup()
    {
        Item _item = null;

        Ray _ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit, 2.2f, 8 | 9))
        {
            _item = _hit.transform.GetComponentInParent<Item>();
        }

        if (_item)
            HoverInfoText.text = _item.myItem.title;
        else
            HoverInfoText.text = "";
    }

    void EquipInfo()
    {
        if (!PA.EquipedItem)
        {
            curItemInfoText.text = "";
            return;
        }

        curItemInfoText.text = "R to keep: " + PA.EquipedItem.title;
    }

    void ControlsInfo()
    {
        if (!CL.IM.Help())
        {
            return;
        }

        ControlsInfoText.gameObject.SetActive(!ControlsInfoText.gameObject.activeSelf);
    }

}
