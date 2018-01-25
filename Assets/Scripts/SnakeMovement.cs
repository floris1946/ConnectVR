using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour {

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


    void Update()
    {

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
        //GetComponent<Rigidbody>().AddForce((force * Speed));
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, currentDestination, Time.deltaTime * Speed);

        float amplitude = 4;
        float frequency = 0.25f;

        // wiggle
        transform.position += amplitude * (Mathf.Sin(2 * Mathf.PI * frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - Time.deltaTime))) * transform.up;


        Rotation();

        //Debug.DrawLine(transform.position, currentDestination);

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

    private float RandomAngle()
    {
        return Random.Range(-40f, 40f);
    }

    private float InterpolatedAngle(float current, float target)
    {
        return Mathf.MoveTowardsAngle(current, target, Speed * Time.deltaTime);
    }

    private Vector3 NewDirection()
    {
        return new Vector3(InterpolatedAngle(transform.eulerAngles.x, RandomAngle()), InterpolatedAngle(transform.eulerAngles.y, RandomAngle()), InterpolatedAngle(transform.eulerAngles.z, RandomAngle()));
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
