using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour {

    // max distance the snake gets from player
    public float distance;

    public float Speed; 

    private float time;
    private float randomPeriod;

    private Vector3 currentDestination;

    private PointerProperties pointerState1;
    private PointerProperties pointerState2;

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

    // Update is called once per frame
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

    }

    // Set the random period before the destination changes
    private void SetRandomPeriod()
    {
        randomPeriod = Random.Range(8f, 16f);
    }

    // Set random new destination within specified boundaries
    private void SetNewDestination()
    {
        Vector3 center = SteamVR_Render.Top().camera.transform.position;
        Vector3 randomLocation = Random.insideUnitSphere;
        Vector3 newDestination = center + randomLocation * distance;

        gameObject.transform.LookAt(newDestination);

        currentDestination = newDestination;


        //Debug.Log("new random destination: " + currentDestination);
    }

    // Set new destination with random z value
    public void SetNewDestination(float x, float y)
    {
        //currentDestination = new Vector3(x, y, Random.Range(minZ, maxZ));
       // Debug.Log("new destination: " + currentDestination);
    }

    // Set new destination with random z value
    public void SetNewDestination(Vector2 point)
    {
        SetNewDestination(point.x, point.y);
    }

    public void SetBoundaries()
    {
        


    }
}
