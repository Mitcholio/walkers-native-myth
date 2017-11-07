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
    public List<Animal> nearbyAnimals = new List<Animal>();
    public List<Animal> detectedAnimals = new List<Animal>();
    SphereCollider AnimalFindCol;

    //Pickup item vars
    public Transform Hand;
    public GameObject EquipedItemGO;
    public ItemProperties EquipedItem;

    System.Random r = new System.Random();

    LayerMask PlayerMask;

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
        ActFlashLight();

        if(EquipedItem)
            ActDropItem();
        else
            ActPickUpItem();
        
        ActKeepItem();
	}

    void FixedUpdate()
    {
        CheckForAnimals();
    }

    void ActFlashLight()
    {
        if (CL.IM.Flashlight())
        {
            UnDetectAnimals();
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

    void ActPickUpItem()
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
            }
        }
    }

    void ActKeepItem()
    {
        if (!CL.IM.KeepItem() || !EquipedItem)
            return;

        UnEquipItem();
    }

    void ActDropItem()
    {
        if (!EquipedItem || !CL.IM.Pickup())
            return;

        Destroy(EquipedItemGO);

        if (PlaceEquipedItem())
            return;

        DropEquipedItem();
    }

    void DropEquipedItem()
    {
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

    bool PlaceEquipedItem()
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

    public void DropItem(ItemProperties _itemProp)
    {
        GameObject _temp = new GameObject();
        Item _item = _temp.AddComponent<Item>();
        _item.myItem = _itemProp;

        _temp.transform.position = Hand.position;
        _temp.transform.rotation = Hand.rotation;

        Rigidbody rb = _temp.GetComponent<Rigidbody>();
        StartCoroutine(_item.AddForce(cam.transform.forward * 0.25f));

        Inv.RemItem(_itemProp);
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
        if (nearbyAnimals.Count < 1)
            return;

        foreach (Animal animal in nearbyAnimals)
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(animal.transform.position);
            if (screenPoint.z > 0 && screenPoint.x > 0.35f && screenPoint.x < 0.65f && screenPoint.y > 0.3f && screenPoint.y < 0.65f)
            {
                if(FL_on)
                    AnimalDetected(animal);
            }
            else
            {
                    AnimalNotDetected(animal);
            }
        }
    }

    void UnDetectAnimals()
    {
        foreach (Animal animal in detectedAnimals)
        {
            animal.OnNotDetected();
        }
        detectedAnimals = new List<Animal>();
    }

    void AnimalDetected(Animal animal)
    {
        if (!detectedAnimals.Contains(animal))
        {
            detectedAnimals.Add(animal);
            animal.OnDetected();
        }
    }

    void AnimalNotDetected(Animal animal)
    {
        if (detectedAnimals.Contains(animal))
        {
            detectedAnimals.Remove(animal);
            animal.OnNotDetected();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Animal animal = other.GetComponentInParent<Animal>();
        if (animal)
        {
            if (!nearbyAnimals.Contains(animal))
                nearbyAnimals.Add(animal);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Animal animal = other.GetComponentInParent<Animal>();
        if (animal)
        {
            if (nearbyAnimals.Contains(animal))
                nearbyAnimals.Remove(animal);
        }
    }

    public LayerMask GetPlayerMask()
    {
        return PlayerMask;
    }
}
