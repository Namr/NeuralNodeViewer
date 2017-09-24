using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour {


    public enum Mode
    {
        Information,
        Isolation,
        Slicing
    };

    public Transform textTransform;
    public Transform SliceVisualTransform;
    Text text;
    public NodeParser parser;
    public Mode pointerMode = Mode.Information;

	// Use this for initialization
	void Start ()
    {
        text = textTransform.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger) && parser.isIsolating == true)
        {
            parser.isIsolating = false;
        }

        if(pointerMode == Mode.Slicing)
        {
            SliceVisualTransform.gameObject.SetActive(true);
        }
        else
        {
            SliceVisualTransform.gameObject.SetActive(false);
        }

        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            DrawLine(transform.position, transform.position + transform.forward * 1000, Color.green, 0.01f);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                

                if (hit.transform.tag == "Node")
                {
                    char index1 = hit.transform.name[hit.transform.name.Length - 1];
                    char index2 = hit.transform.name[hit.transform.name.Length - 2];
                    char index3 = hit.transform.name[hit.transform.name.Length - 3];

                    int index;
                    if(char.IsNumber(index2))
                    {
                        if(char.IsNumber(index3))
                        {
                            index = int.Parse(index3.ToString() + index2.ToString() + index1.ToString());
                        }
                        else
                        {
                            index = int.Parse(index2.ToString() + index1.ToString());
                        }
                            
                    }
                    else
                    {
                        index = int.Parse(index1.ToString());
                    }
                    if(pointerMode == Mode.Information)
                    {
                        text.text = hit.transform.name;
                    }
                    else if(pointerMode == Mode.Isolation)
                    {
                        parser.isIsolating = true;
                        parser.isolatedNode = index;
                    }
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

    public void ChangeMode(int newMode)
    {
        switch (newMode)
        {
            case 0:
                pointerMode = Mode.Information;
                break;
            case 1:
                pointerMode = Mode.Isolation;
                break;
            case 2:
                pointerMode = Mode.Slicing;
                break;
        }

    }
}
