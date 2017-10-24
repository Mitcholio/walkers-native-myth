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
        RaycastHit[] _hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 2.2f);
        foreach (RaycastHit _hit in _hits)
        {
            if (_hit.collider.GetComponentInParent<Item>())
            {
                _item = _hit.collider.GetComponentInParent<Item>();
            }
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

        curItemInfoText.text = "R to save: " + PA.EquipedItem.title;
    }

    void ControlsInfo()
    {
        if (!CL.IM.help)
        {
            return;
        }

        ControlsInfoText.gameObject.SetActive(!ControlsInfoText.gameObject.activeSelf);
    }

}
