using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour {

    private List<GameObject> soundbars;

	// Use this for initialization
	void Start () {
        soundbars = gameObject.GetComponent<RhythmVisualizatorPro>().soundBars;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 nextPos = soundbars[0].transform.position;

        for(int i = 0; i < soundbars.Count; i++)
        {

            Vector3 temp = soundbars[i].transform.position;


            // update direction
            Vector3 direction = nextPos - temp;

            if (i != 0)
            {
                // update position
                soundbars[i].transform.position = nextPos;
                nextPos = temp;

           

                if (i == 1)
                {
                    Debug.Log(direction);
                    Debug.DrawRay(soundbars[i].transform.position, direction * 100, Color.red);
                }

                if (direction.x == 0)
                    direction.x = 1e-8f;
                if (direction.y == 0)
                    direction.y = 1e-8f;
                if (direction.z == 0)
                    direction.z = 1e-8f;

                float alphaX = Mathf.Atan(direction.y / direction.z) * Mathf.Rad2Deg;
                float alphaY = Mathf.Atan(direction.z / direction.x) * Mathf.Rad2Deg;
                float alphaZ = Mathf.Atan(direction.y / direction.x) * Mathf.Rad2Deg;

                Debug.Log(alphaX + " " + alphaY + " " + alphaZ);

                soundbars[i].transform.rotation = Quaternion.Euler(alphaX, alphaY, alphaZ);
            }

        }
		
	}
}
