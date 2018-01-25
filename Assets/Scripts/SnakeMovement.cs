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
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, currentDestination, Time.deltaTime * Speed);
        Rotation();

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

        Vector3 randomLocation = Random.insideUnitSphere;
        Vector3 newDestination = center + randomLocation * Distance;

        currentDestination = newDestination;
    }

    void Rotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    }
}
