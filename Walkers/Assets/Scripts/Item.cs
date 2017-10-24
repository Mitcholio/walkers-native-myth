using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public ItemProperties myItem;
    public GameObject model;

    Rigidbody rb;

	// Use this for initialization
	void Start ()
    {
        StartVars();
        SpawnModel();
        SetName();
	}

    void StartVars()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		
	}

    void SetName()
    {
        if (!myItem)
            return;

        transform.name = myItem.title + "_Item";
    }

    //Model can be set manually in the inspector, in which case no other model will be spawned,
    //except when the Item is dropped by player.
    void SpawnModel()
    {
        if (!myItem.model || model)
            return;

        model = Instantiate(myItem.model, transform);
        model.transform.position = transform.position;
        MeshCollider MC = model.GetComponentInChildren<Renderer>().gameObject.AddComponent<MeshCollider>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        MC.convex = true;
    }
}
