using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour {

    [SerializeField]
    private Transform startPosition;
    [SerializeField]
    private Transform endPosition;

    public float speed = 2.5f;
    public float currentRotation;
    public float rotationSensitivity = 50.0f;

    public float Distance;
    public float Speed;

    private float time;
    private float randomPeriod;

    private Vector3 force;
    private Vector3 currentDestination;

    private PointerProperties pointerState1;
    private PointerProperties pointerState2;

    public List<Transform> bodyParts = new List<Transform>();

    private float timer;
    [SerializeField]
    private float endTime;
    [SerializeField]
    private float beginSpeed;
    private bool begin = true;

    // Use this for initialization
    void Start()
    {
        pointerState1 = GameObject.Find("LeftController").GetComponent<PointerProperties>();
        pointerState2 = GameObject.Find("RightController").GetComponent<PointerProperties>();
        // Set current destination to current position
        currentDestination = gameObject.transform.position;

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

    void Update()
    {
        timer += Time.deltaTime;
        if (begin && Vector3.Distance(transform.position, currentDestination) > 20)
        {
            MoveTowardsStartPosition();
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, beginSpeed * Time.deltaTime);
        }
        else if (timer > endTime)
        {
            MoveTowardsEndPosition();
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, beginSpeed * Time.deltaTime);
        }
        else
        {
            begin = false;
            if (pointerState1.pointer.IsPointerActive())
            {
                currentDestination = pointerState1.pRenderer.GetPointerObjects()[1].transform.position;
                time = 0;
            }
            else if (pointerState2.pointer.IsPointerActive())
            {
                currentDestination = pointerState2.pRenderer.GetPointerObjects()[1].transform.position;
                time = 0;
            }

            else
            {
                time += Time.deltaTime;

                if (time >= randomPeriod)
                {
                    SetNewDestination();
                    SetRandomPeriod();
                    time = 0f;
                }
            }

            GetComponent<Rigidbody>().AddRelativeForce((currentDestination - transform.position).normalized * 0.8f, ForceMode.Force);
        }

        float amplitude = 4;
        float frequency = 0.25f;

        // wiggle
        transform.position += amplitude * (Mathf.Sin(2 * Mathf.PI * frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - Time.deltaTime))) * transform.up;


        Rotation();
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
        randomPeriod = Random.Range(0.2f, 3f);
    }

    // Set new random destination
    private void SetNewDestination()
    {
        Vector3 center;

        if (SteamVR_Render.Top() == null)
            center = Vector3.zero;
        else
            center = SteamVR_Render.Top().camera.transform.position;

        Vector3 randomLocation = GetPointOnUnitSphereCap(currentDestination, 20);//Random.insideUnitSphere;
        Vector3 newDestination = center + randomLocation * Distance;

        currentDestination = newDestination;

        //force += Quaternion.AngleAxis(Random.Range(-30f, 30f), Vector3.forward) * Vector3.one;
    }

    void Rotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    }
}
