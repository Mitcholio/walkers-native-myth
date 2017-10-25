using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    Camera cam;
    Inventory Inv;
    ComponentList CL;

    //Flashlight vars
    public float maxBattery;
    public float curBattery;
    public float FL_range;
    public bool FL_on = false;
    float flickerTimer;
    Light flashLight;

    //Animal vars
    public List<GameObject> nearbyAnimals = new List<GameObject>(); //<GameObject> to be replaced by <Animal> class
    SphereCollider AnimalFindCol;

    //Pickup item vars
    public Transform Hand;
    public GameObject EquipedItemGO;
    public ItemProperties EquipedItem;

    System.Random r = new System.Random();

    public LayerMask PlayerMask;

    // Use this for initialization
    void Start ()
    {
        StartVars();
        SetAnimalFindCol();
	}

    void StartVars()
    {
        CL = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<ComponentList>();
        flashLight = GetComponentInChildren<Light>();
        flashLight.enabled = false;
        flashLight.range = FL_range;
        curBattery = maxBattery;
        cam = Camera.main;
        Inv = GetComponent<Inventory>();
        PlayerMask += 8 | 9;
    }

    void SetAnimalFindCol()
    {
        AnimalFindCol = gameObject.AddComponent<SphereCollider>();
        AnimalFindCol.radius = FL_range;
        AnimalFindCol.isTrigger = true;
    }

    // Update is called once per frame
    void Update ()
    {
        FlashLight();

        if(EquipedItem)
            DropItem();
        else
            PickUpItem();
        
        KeepItem();
	}

    void FixedUpdate()
    {
        CheckForAnimals();
    }

    void FlashLight()
    {
        if (CL.IM.Flashlight())
        {
            toggleFlashLight(!FL_on);
        }

        if (FL_on)
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
        FL_on = _state;
    }

    void FlashLightFlicker()
    {
        float _temp = r.Next(6, 30) * 0.01f;
        if (Time.time < flickerTimer + _temp)
            return;

        flickerTimer = Time.time;

        toggleFlashLight(!FL_on);
    }

    void PickUpItem()
    {
        if (CL.IM.Pickup())
        {
            Item _item = null;

            Ray _ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, 2.2f, PlayerMask))
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

        if (Physics.Raycast(_ray, out _hit, 2.2f, PlayerMask))
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
        Destroy(EquipedItemGO);

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
        if (EquipedItem)
        {
            Destroy(EquipedItemGO);
            EquipedItem = null;
        }
    }

    void CheckForAnimals()
    {
        if (nearbyAnimals.Count < 1 || !FL_on)
            return;

        foreach (GameObject animal in nearbyAnimals)
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(animal.transform.position);
            if (screenPoint.z > 0 && screenPoint.x > 0.35f && screenPoint.x < 0.65f && screenPoint.y > 0.35f && screenPoint.y < 0.65f)
            {
                AnimalDetected(animal);
            }
        }
    }

    void AnimalDetected(GameObject animal)
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Animal")
        {
            nearbyAnimals.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Animal")
        {
            nearbyAnimals.Remove(other.gameObject);
        }
    }
}
