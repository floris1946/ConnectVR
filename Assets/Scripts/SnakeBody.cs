using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour {

    private int myOrder;
    private Transform head;

    private Vector3 movementVelocity;
    [Range(0.0f, 1.0f)]
    public float overTime = 0.5f;

	// Use this for initialization
	void Start () {
        head = GameObject.Find("SnakeHead").transform;
        for (int i = 0; i < head.GetComponent<SnakeMovement>().bodyParts.Count; i++)
        {
            if (gameObject == head.GetComponent<SnakeMovement>().bodyParts[i].gameObject)
            {
                myOrder = i;
            }
        }
	}

    private void FixedUpdate()
    {
        if (myOrder == 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, head.position, ref movementVelocity, overTime);
            transform.LookAt(head.transform.position);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, head.GetComponent<SnakeMovement>().bodyParts[myOrder - 1].position, ref movementVelocity, overTime);
            transform.LookAt(head.GetComponent<SnakeMovement>().bodyParts[myOrder - 1].position);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
