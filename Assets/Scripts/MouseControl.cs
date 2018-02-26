using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour {

    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;
    public Transform brain;

    // Use this for initialization
    void Start () {
		
	}


    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            float h = horizontalSpeed * Input.GetAxis("Mouse X");
            float v = verticalSpeed * Input.GetAxis("Mouse Y");
            brain.Rotate(v, 0, h);
        }
    }
}
