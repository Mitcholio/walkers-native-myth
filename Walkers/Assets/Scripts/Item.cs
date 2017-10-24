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
        SetName();
        SpawnModel();
	}

    void StartVars()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.mass = 0.1f;
        rb.drag = 3;
        rb.angularDrag = 3;
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
        if (!myItem.model)
            return;

        if (!model)
        {
            model = Instantiate(myItem.model, transform);
            model.transform.position = transform.position;
        }

        MeshCollider MC = model.GetComponentInChildren<Renderer>().gameObject.AddComponent<MeshCollider>();
        MC.convex = true;
    }

    public IEnumerator AddForce(Vector3 _dir)
    {
        yield return new WaitForFixedUpdate();
        rb.AddForce(_dir, ForceMode.Impulse);
    }
}
