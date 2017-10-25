using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    ComponentList CL;
    InputManager IM;
    Rigidbody rb;
    
    public float moveSpeed;
    public float CrouchMultiplier;
    public float SprintMultiplier;
    public float AirMultiplier;
    public float jumpForce;
    public float jumpUpDuration;
    public float gravity;
    public float stepForce;
    public float stepHeight;
    public float playerHeight;
    public float maxAngle;

    float floorAngleMulti = 0.15f;

    Vector3 moveDir = Vector3.zero;

    public Transform EyePoint;
    public Transform UB_Col;

    public bool Grounded = false;
    bool crouched = false;
    float crouchMulti = 1f;
    float airMulti = 1;
    Vector3 eyePos = Vector3.zero;
    Vector3 curEyePos = Vector3.zero;
    bool doCrouchLerp = false;

    float h = 0;
    float v = 0;

    float playerRadius = 0.2f;
    float gFootHeight = 0.1f;
    List<Vector3> gFeet = new List<Vector3>();

    float stairCD = 0;

    float moveCD = 0;

    // Use this for initialization
    void Start()
    {
        StartVars();
        GroundFeet();
    }

    void StartVars()
    {
        Application.runInBackground = true;
        CL = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<ComponentList>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        //EyePoint.transform.localPosition = new Vector3(EyePoint.transform.localPosition.x, playerHeight - 0.05f, EyePoint.transform.localPosition.z);
        //UB_Col.transform.localPosition = new Vector3(UB_Col.transform.localPosition.x, playerHeight - 0.3f, UB_Col.transform.localPosition.z);

        eyePos = EyePoint.transform.localPosition;
        IM = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<InputManager>();

        rb.mass = 1; rb.drag = 1; rb.angularDrag = 1; rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGround();
        Move();

        if (doCrouchLerp)
            CrouchLerp();

        ClimbStair();
    }

    void Update()
    {
        MoveInput();
        Jump();
        Crouch();
    }

    public void MoveInput()
    {
        h = IM.moveH;
        v = IM.moveV;
    }

    public void Move()
    {
        moveDir = new Vector3(h, 0, v);
        moveDir = transform.TransformDirection(moveDir);
        moveDir.Normalize();

        float _sprintMulti = 1;
        if (IM.Sprint())
            _sprintMulti = SprintMultiplier;

        moveDir = moveDir * moveSpeed * crouchMulti * airMulti * _sprintMulti * Time.deltaTime * 80;

        float floorAngle = getFloorAngle();
        float slopeAngle = getSlopeAngle();

        if (slopeAngle > maxAngle)
        {
            moveDir *= 0.3f;
            moveDir.y = -gravity * Time.deltaTime;
        }
        else
        {
            if (floorAngle < 0)
            {
                moveDir = moveDir * 0.75f;
                moveDir.y += floorAngle * floorAngleMulti;
            }
            if (slopeAngle > maxAngle * 0.5f)
            {
                moveDir = moveDir * 0.65f;
            }
            if (slopeAngle > maxAngle * 0.75f)
            {
                moveDir = moveDir * 0.55f;
            }

            if (!Grounded)
                moveDir.y = moveDir.y - gravity * Time.deltaTime;

            if (Time.time < moveCD + jumpUpDuration)
            {
                moveDir = rb.velocity;
            }
        }

        Vector3 _vel = rb.velocity;
        Vector3 _vel3 = Vector3.Lerp(_vel, moveDir, 6f * Time.deltaTime);
        rb.velocity = _vel3;
    }

    public bool V3Equal(Vector3 a, Vector3 b, float n)
    {
        return Vector3.SqrMagnitude(a - b) < n;
    }

    public float getFloorAngle()
    {
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + gFootHeight, transform.position.z), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.4f))
        {
            if (hit.collider.gameObject.transform.tag != "Player")
            {
                if (v > 0)
                {
                    float angle = Vector3.Angle(hit.normal, transform.forward) - 90;
                    return angle;
                }
                else if (v < 0)
                {
                    float angle = Vector3.Angle(hit.normal, -transform.forward) - 90;
                    return angle;
                }
                else if (h > 0)
                {
                    float angle = Vector3.Angle(hit.normal, transform.right) - 90;
                    return angle;
                }
                else if (h < 0)
                {
                    float angle = Vector3.Angle(hit.normal, -transform.right) - 90;
                    return angle;
                }
            }
        }
        return 0;
    }

    public float getSlopeAngle()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.4f))
        {
            if (hit.collider.gameObject.transform.tag != "Player")
            {
                float angle = Vector3.Angle(hit.normal, -transform.up) - 180;
                angle = Mathf.Abs(angle);
                return angle;
            }
        }
        return 0;
    }

    void Jump()
    {
        if (!Grounded || CheckRoof())
            return;

        if (IM.Jump())
        {
            float _tempForce = jumpForce;
            if (getSlopeAngle() > 40)
                _tempForce = jumpForce * 0.5f;
            rb.AddForce(Vector3.up * _tempForce, ForceMode.Impulse);

            moveCD = Time.time;
        }
    }

    void Crouch()
    {
        if (IM.Crouch())
        {
            if (crouched && !CheckRoof())
            {
                crouched = false;
                crouchMulti = 1;
                curEyePos = eyePos;
                doCrouchLerp = true;
            }
            else
            {
                crouched = true;
                crouchMulti = CrouchMultiplier;
                curEyePos = new Vector3(eyePos.x, eyePos.y - 0.8f, eyePos.z);
                doCrouchLerp = true;
            }
        }
    }

    void CrouchLerp()
    {
        float _temp = Mathf.Lerp(EyePoint.localPosition.y, curEyePos.y, 5 * Time.deltaTime);
        EyePoint.localPosition = new Vector3(0, _temp, eyePos.z);
        UB_Col.localPosition = new Vector3(0, _temp - 0.3f, 0);

        if (_temp < EyePoint.localPosition.y - 0.05f)
        {
            doCrouchLerp = false;
        }
    }

    void CheckGround()
    {
        Grounded = false;
        airMulti = AirMultiplier;
        foreach (Vector3 _foot in gFeet)
        {
            Vector3 _foot2 = _foot + transform.position;
            Ray _ray = new Ray(_foot2, Vector3.down);
            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit, gFootHeight * 1.5f))
            {
                if (!_hit.collider.GetComponentInParent<PlayerController>())
                {
                    Grounded = true;
                    airMulti = 1;
                }
                break;
            }
        }
    }

    void GroundFeet()
    {
        Vector3 _gFoot1 = new Vector3(0, gFootHeight, 0);
        Vector3 _gFoot2 = new Vector3(-playerRadius, gFootHeight, playerRadius);
        Vector3 _gFoot3 = new Vector3(playerRadius, gFootHeight, playerRadius);
        Vector3 _gFoot4 = new Vector3(playerRadius, gFootHeight, -playerRadius);
        Vector3 _gFoot5 = new Vector3(-playerRadius, gFootHeight, -playerRadius);

        gFeet.Add(_gFoot1);
        gFeet.Add(_gFoot2);
        gFeet.Add(_gFoot3);
        gFeet.Add(_gFoot4);
        gFeet.Add(_gFoot5);
    }

    void ClimbStair()
    {
        if (stairCD+0.4f > Time.time) //
            return;

        if (IM.movementSqrMag() < 0.1)
            return;

        float _h = 0;
        float _v = 0;

        if (h > 0.1f)
        {
            _h = playerRadius + 0.2f;
        }
        if (h < -0.1f)
        {
            _h = -playerRadius - 0.2f;
        }
        if (v > 0.1f)
        {
            _v = playerRadius + 0.2f;
        }
        if (v < -0.1f)
        {
            _v = -playerRadius - 0.2f;
        }

        Vector3 _pos = transform.position;
        _pos += transform.right * _h;
        _pos.y += stepHeight;
        _pos += transform.forward * _v;

        Ray _ray = new Ray(_pos, Vector3.down);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit, stepHeight * 0.5f))
        {
            if (_hit.collider.tag == "Stairs")
            {
                //rb.velocity = new Vector3(rb.velocity.x, stepForce * Time.deltaTime, rb.velocity.z);
                rb.AddForce(new Vector3(0, stepForce * stepHeight, 0), ForceMode.Impulse);

                stairCD = Time.time; //
            }
        }
    }

    bool CheckRoof()
    {
        bool _r = false;
        if (!crouched)
            return _r;

        Ray _ray = new Ray(transform.position, Vector3.up);

        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit, playerHeight, CL.PA.PlayerMask))
        {
            _r = true;
        }

        return _r;
    }

}
