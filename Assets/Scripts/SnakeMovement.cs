using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour {

    [SerializeField]
    private Transform startPosition;
    [SerializeField]
    private Transform endPosition;
    
    public float currentRotation;
    public float rotationSensitivity = 50.0f;
    public float Distance;

    private float time;
    private float randomPeriod;
    [SerializeField]
    private float amplitude = 2;
    [SerializeField]
    private float frequency = 0.25f;
    [SerializeField]
    private float wanderSpeed = 0.6f;
    [SerializeField]
    private float followSpeed = 1f;

    private Vector3 force;
    private Vector3 currentDestination;

    private PointerProperties pointerState1;
    private PointerProperties pointerState2;

    private bool isPointerActivePrev1, isPointerActivePrev2;

    public List<Transform> bodyParts = new List<Transform>();

    private float timer;
    [SerializeField]
    private float endTime;
    [SerializeField]
    private float beginSpeed;
    private float beginSpeedTransition;
    private bool begin = true;

    private bool isPointerPressed
    {
        get
        {
            if (!isPointerActivePrev1 && pointerState1.pointer.IsPointerActive())
                return true;

            if (!isPointerActivePrev2 && pointerState2.pointer.IsPointerActive())
                return true;

            return false;
        }
    }

    // Use this for initialization
    void Start()
    {
        pointerState1 = GameObject.Find("LeftController").GetComponent<PointerProperties>();
        pointerState2 = GameObject.Find("RightController").GetComponent<PointerProperties>();
        // Set current destination to current position
        currentDestination = gameObject.transform.position;
        MoveTowardsStartPosition();
        beginSpeedTransition = beginSpeed;
        time = 0f;
        SetRandomPeriod();
    }

    private void MoveTowardsStartPosition()
    {
        currentDestination = startPosition.position;
    }
    private void MoveTowardsEndPosition()
    {
        currentDestination = endPosition.position;
    }

    void FixedUpdate()
    {
        float speed = wanderSpeed;

        Debug.Log(beginSpeedTransition);

        if (begin && Vector3.Distance(transform.position, currentDestination) > 20)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, beginSpeedTransition * Time.deltaTime);
            //beginSpeedTransition = Mathf.Lerp(beginSpeed, speed, /* todo */ );
        }
        else if (timer > endTime)
        {
            MoveTowardsEndPosition();
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, beginSpeedTransition * Time.deltaTime);
           // beginSpeedTransition = Mathf.Lerp(speed, beginSpeed, Time.deltaTime);
        }
        else
        {
            speed = followSpeed;
            begin = false;
            if (pointerState1.pointer.IsPointerActive())
            {
                currentDestination = pointerState1.pRenderer.GetPointerObjects()[1].transform.position;
                time = 0;

                Debug.DrawLine(pointerState1.pRenderer.GetPointerObjects()[1].transform.position, transform.position, Color.green);
                
            }
            else if (pointerState2.pointer.IsPointerActive())
            {
                currentDestination = pointerState2.pRenderer.GetPointerObjects()[1].transform.position;
                time = 0;

                Debug.DrawLine(pointerState2.pRenderer.GetPointerObjects()[1].transform.position, transform.position, Color.green);
                
            }
            else
            {
                speed = wanderSpeed;
                time += Time.deltaTime;

                if (time >= randomPeriod)
                {
                    SetNewDestination();
                    SetRandomPeriod();
                    time = 0f;
                }
            }

            Debug.DrawLine(Vector3.zero, currentDestination, Color.blue);

            if (isPointerPressed)
            {
                Debug.Log("pointer pressed");
                // make a turn towards the pointer
                GetComponent<Rigidbody>().velocity = Vector3.Normalize(currentDestination) * speed;
            }

            // Wandering
            GetComponent<Rigidbody>().AddRelativeForce((currentDestination - transform.position).normalized * speed, ForceMode.Force);
        }

        // Sine movement
        transform.position += amplitude * (Mathf.Sin(2 * Mathf.PI * frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - Time.deltaTime))) * transform.up;


        Rotation();


        isPointerActivePrev1 = pointerState1.pointer.IsPointerActive();
        isPointerActivePrev2 = pointerState2.pointer.IsPointerActive();

    }

    public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
    {
        var angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        var PointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        var V =  new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
        return targetDirection * V;
    }
    public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
    {
        return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
    }

    // Set a random period before a new destination is chosen
    private void SetRandomPeriod()
    {
        randomPeriod = Random.Range(5f, 8f);
    }

    // Set new random destination
    private void SetNewDestination()
    {
        Vector3 center;

        if (SteamVR_Render.Top() == null)
            center = Vector3.zero;
        else
            center = SteamVR_Render.Top().camera.transform.position;

        Vector3 randomLocation = GetPointOnUnitSphereCap(currentDestination, 20);
        Vector3 newDestination = center + randomLocation * Distance;

        currentDestination = newDestination;
    }

    void Rotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    }
}
