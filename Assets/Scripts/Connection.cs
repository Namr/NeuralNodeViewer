using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour {
    public float threshold;
    public NodeParser parser;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(this.transform.parent.gameObject.activeSelf)
        {
            if (parser.threshold > threshold)
            {
                //this.gameObject.SetActive(false);
            }
            else
            {
                //this.gameObject.SetActive(true);
            }
        }

	}
}
