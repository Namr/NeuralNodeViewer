using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainMeshSlicing : MonoBehaviour {


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    
    public void Slice()
    {
        MeshFilter mf = transform.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;

    }
}
