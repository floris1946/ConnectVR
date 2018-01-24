using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SetTargetPosition : MonoBehaviour {

    public VRTK_Pointer pointer;
    public VRTK_StraightPointerRenderer pRenderer;

    // Use this for initialization
    void Start()
    {
        pointer = GetComponent<VRTK_Pointer>();
        pRenderer = GetComponent<VRTK_StraightPointerRenderer>();
    }

    // Update is called once per frame
    void Update ()
    { 
    }
}
