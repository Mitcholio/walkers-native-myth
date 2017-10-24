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
        SaveItem();
	}

    void FlashLight()
    {
        if (CL.IM.flashlight)
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
        if (CL.IM.pickup)
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
            {
                Inv.PickUp(_item);
                EquipItem(0, _item.myItem);
            }
        }
    }

    void SaveItem()
    {
        if (!CL.IM.saveItem || !EquipedItem)
            return;

        UnEquipItem();
    }

    void DropItem()
    {
        if (!EquipedItem)
            return;

        if (!CL.IM.pickup)
            return;

        if (EquipedItemGO)
        {
            Destroy(EquipedItemGO);

            GameObject _temp = Instantiate(Inv.ItemPrefab);
            Item _item = _temp.GetComponent<Item>();
            _item.myItem = EquipedItem;

            _temp.transform.position = Hand.position;
            _temp.transform.rotation = Hand.rotation;

            Rigidbody rb = _temp.GetComponent<Rigidbody>();
            rb.AddForce(cam.transform.forward * 2, ForceMode.Impulse);

            Inv.RemItem(EquipedItem);
            EquipedItem = null;
        }
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
