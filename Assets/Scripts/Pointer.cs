using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour {

    public Transform textTransform;
    Text text;

	// Use this for initialization
	void Start ()
    {
        text = textTransform.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {

            DrawLine(transform.position, transform.position + transform.forward * 1000, Color.green, 0.2f);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.transform.tag == "Node")
                {
                    text.text = hit.transform.name;
                }
            }
        }
        else
        {
            text.text = "";
        }
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float width)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        myLine.transform.parent = this.transform;
        Destroy(myLine,0.05f);
    }
}
