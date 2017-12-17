using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolProp : MonoBehaviour {

    public Pointer pointer;
    public Pointer.Mode mode;

    private MeshRenderer renderer;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    void Update ()
    {
		if(pointer.pointerMode == mode)
        {
            renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
        }
	}
}
