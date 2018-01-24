using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{
    public float distance;

    public float speed;

    private float time;
    private float randomPeriod;

    private Vector3 currentDestination;

    private void Start()
    {
        time = 0f;
        SetRandomPeriod();
    }
    private void Update()
    {
        time += Time.deltaTime;

        if (time >= randomPeriod)
        {
            SetNewDestination();
            SetRandomPeriod();
            time = 0f;
        }

        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, currentDestination, Time.deltaTime * speed);
    }

    private void SetRandomPeriod()
    {
        randomPeriod = Random.Range(5f, 8f);
    }

    private void SetNewDestination()
    {
        Vector3 center;
        if (SteamVR_Render.Top() == null)
            center = Vector3.zero;
        else
            center = SteamVR_Render.Top().camera.transform.position;
        Vector3 randomLocation = Random.insideUnitSphere;
        Vector3 newDestination = center + randomLocation * distance;

        gameObject.transform.LookAt(newDestination);

        currentDestination = newDestination;
    }
}
