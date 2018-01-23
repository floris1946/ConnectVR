using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour {

    public List<Transform> Bodyparts = new List<Transform>();

    public float mindistance = 0.25f;

    public int beginsize;

    public float speed = 1;
    public float rotationspeed = 50;

    public GameObject bodyprefab;

    private float dis;
    private Transform curBodyPart;
    private Transform prevBodyPart;

    // Use this for initialization
    void Start() {
        for (int i = 0; i < beginsize - 1; i++)
        {
            AddBodyPart();
        }
    }

    // Update is called once per frame
    void Update() {
        Move();

        if (Input.GetKey(KeyCode.Q))
            AddBodyPart();
    }

    public void Move()
    {
        float curspeed = speed;
        if (Input.GetKey(KeyCode.W))
            curspeed *= 2;
        Bodyparts[0].Translate(Bodyparts[0].forward * curspeed * Time.smoothDeltaTime, Space.World);

        if (Input.GetAxis("Horizontal") != 0)
        {
            Bodyparts[0].Rotate(Vector3.up * rotationspeed * Time.deltaTime * Input.GetAxis("Horizontal"));
        }

        for (int i = 1; i < Bodyparts.Count; i++)
        {
            curBodyPart = Bodyparts[i];
            prevBodyPart = Bodyparts[i = 1];

            dis = Vector3.Distance(prevBodyPart.position, curBodyPart.position);

            Vector3 newpos = prevBodyPart.position;

            newpos.y = Bodyparts[0].position.y;

            float T = Time.deltaTime * dis / mindistance * curspeed;

            if (T > 0.5f)
            {
                T = 0.5f;
            }
            curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);
            curBodyPart.rotation = Quaternion.Slerp(curBodyPart.rotation, prevBodyPart.rotation, T);
        }
    }

    public void AddBodyPart()
    {
        Transform newpart = GameObject.Instantiate(bodyprefab, Bodyparts[Bodyparts.Count - 1].position, Bodyparts[Bodyparts.Count - 1].rotation).transform;
        newpart.SetParent(transform);
        Bodyparts.Add(newpart);
    }
}
