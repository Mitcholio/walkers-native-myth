using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    Camera cam;
    public Inventory Inv;
    ComponentList CL;
    public float maxBattery;
    public float curBattery;
    public float range;
    public bool on = false;

    Light flashLight;

    public Transform Hand;
    public GameObject EquipedItemGO;
    public ItemProperties EquipedItem;

    float flickerTimer;
    System.Random r = new System.Random();

    // Use this for initialization
    void Start ()
    {
        StartVars();
	}

    void StartVars()
    {
        CL = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<ComponentList>();
        flashLight = GetComponentInChildren<Light>();
        flashLight.enabled = false;
        flashLight.range = range;
        curBattery = maxBattery;
        cam = Camera.main;
        Inv = GetComponent<Inventory>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        FlashLight();

        DropItem();
        PickUpItem();
        KeepItem();
	}

    void FlashLight()
    {
        if (CL.IM.Flashlight())
        {
            toggleFlashLight(!on);
        }

        if (on)
            curBattery -= Time.deltaTime;

        if (curBattery < 0)
        {
            toggleFlashLight(false);
            return;
        }

        if (curBattery < 1f)
        {
            FlashLightFlicker();
        }
    }

    void toggleFlashLight(bool _state)
    {
        if (curBattery < 0)
        {
            _state = false;
        }

        flashLight.enabled = _state;
        on = _state;
    }

    void FlashLightFlicker()
    {
        float _temp = r.Next(6, 30) * 0.01f;
        if (Time.time < flickerTimer + _temp)
            return;

        Debug.Log(flickerTimer);
        flickerTimer = Time.time;

        toggleFlashLight(!on);

    }

    void PickUpItem()
    {
        if (CL.IM.Pickup())
        {
            Item _item = null;

            Ray _ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, 2.2f, 8 | 9))
            {
                _item = _hit.collider.GetComponentInParent<Item>();
            }

            if (_item)
            {
                Inv.AddItem(_item.myItem);
                Destroy(_item.gameObject);
                EquipItem(0, _item.myItem);
            }
        }
    }

    void KeepItem()
    {
        if (!CL.IM.KeepItem() || !EquipedItem)
            return;

        UnEquipItem();
    }

    void DropItem()
    {
        if (!EquipedItem || !CL.IM.Pickup())
            return;

        Destroy(EquipedItemGO);

        if (PlaceItem())
            return;

        GameObject _temp = new GameObject();
        Item _item = _temp.AddComponent<Item>();
        _item.myItem = EquipedItem;

        _temp.transform.position = Hand.position;
        _temp.transform.rotation = Hand.rotation;

        Rigidbody rb = _temp.GetComponent<Rigidbody>();
        StartCoroutine(_item.AddForce(cam.transform.forward * 0.25f));

        Inv.RemItem(EquipedItem);
        EquipedItem = null;
    }

    bool PlaceItem()
    {
        Ray _ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit _hit;
        Vector3 _pos;

        if (Physics.Raycast(_ray, out _hit, 2.2f, 8 | 9))
            _pos = _hit.point + Vector3.up * 0.3f;
        else
            return false;

        GameObject _temp = new GameObject();
        Item _item = _temp.AddComponent<Item>();
        _item.myItem = EquipedItem;

        _temp.transform.position = _pos;
        _temp.transform.rotation = Quaternion.identity;

        Inv.RemItem(EquipedItem);
        EquipedItem = null;

        return true;
    }

    public void EquipItem(int _nr, ItemProperties _item)
    {
        if (EquipedItemGO)
        {
            Destroy(EquipedItemGO);
        }

        if(!_item)
        _item = Inv.GetItem(_nr);

        if (!_item)
            return;

        EquipedItem = _item;
        EquipedItemGO = Instantiate(_item.model, Hand);
        EquipedItemGO.transform.localPosition = Vector3.zero;
    }

    public void UnEquipItem()
    {
        if (EquipedItemGO)
        {
            Destroy(EquipedItemGO);
            EquipedItem = null;
        }
    }
}
