using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {

    ComponentList CL;
    CharacterController CC;

    public AnimalProperties animalProperties;
    public float moveSpeed;
    public bool isSkinWalker;

    public GameObject model;
    public Color SW_eyeColor;
    public List<MeshRenderer> eyes = new List<MeshRenderer>();

    float gravity = 4;
    float newDirInterval;
    float newDirTime;
    float targetRotY = 0;
    Vector3 targetRot = Vector3.zero;
    System.Random r;

	// Use this for initialization
	void Start ()
    {
        StartVars();
        SpawnModel();
        AnimalVars();
    }

    void StartVars()
    {
        CL = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<ComponentList>();
        CC = gameObject.AddComponent<CharacterController>();

        int _seed = Random.Range(0, 9999);
        r = new System.Random(_seed);

        targetRotY = r.Next(0, 360);
    }

    void AnimalVars()
    {
        name = animalProperties.title + "_Animal";
        moveSpeed = animalProperties.moveSpeed;
        newDirInterval = r.Next(3, 8);

        CC.radius = animalProperties.size;
        CC.center = new Vector3(0, animalProperties.size, 0);
        CC.height = animalProperties.size;
        CC.slopeLimit = 40;

        foreach (Transform child in model.transform)
        {
            if (child.name.Contains("Eye"))
            {
                MeshRenderer _MR = child.GetComponent<MeshRenderer>();
                eyes.Add(_MR);
            }
        }
    }

    void SpawnModel()
    {
        if (!model)
        {
            model = Instantiate(animalProperties.model, transform);
            model.transform.localPosition = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();

        if (!IsDetected())
            Move();
    }

    void Rotate()
    {
        float _rotSpeed = 1;
        if (IsDetected())
        {
            targetRotY = PlayerDirRot();
            _rotSpeed = 3;
        }
        else
        {
            if (Time.time > newDirTime + newDirInterval)
            {
                newDirInterval = r.Next(3, 8);
                targetRotY = RandomDirRot();
            }
        }

        targetRot.y = Mathf.LerpAngle(transform.eulerAngles.y, targetRotY, _rotSpeed * Time.deltaTime);
        transform.eulerAngles = targetRot;
    }

    void Move()
    {
        CC.Move((transform.forward * moveSpeed * Time.deltaTime) + (Vector3.down * gravity * Time.deltaTime));
    }

    float RandomDirRot()
    {
        newDirTime = Time.time;
        return r.Next(0, 360);
    }

    float PlayerDirRot()
    {
        Quaternion q = Quaternion.LookRotation(CL.PC.transform.position - transform.position);
        return q.eulerAngles.y;
    }

    void SetEyeColor(Color _color)
    {
        foreach (MeshRenderer eye in eyes)
        {
            eye.material.color = _color;
        }
    }

    bool IsDetected()
    {
        return CL.PA.detectedAnimals.Contains(this);
    }

    public void OnDetected()
    {
        Color _color = Color.black;

        if (isSkinWalker)
        {
            _color = SW_eyeColor;
        }

        SetEyeColor(_color);
    }

    public void OnNotDetected()
    {
        SetEyeColor(Color.black);
        if (!isSkinWalker)
            targetRotY = RandomDirRot();
        else
            TransformToSW();
    }

    void TransformToSW()
    {
        Debug.Log("Skinwalker!");
        //SpawnSkinWalker
        //DestroyMe
    }
}
