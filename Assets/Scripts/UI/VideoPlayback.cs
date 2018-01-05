using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayback : MonoBehaviour {


    public Slider slider;
    public NodeParser parser;

    public Transform BrainMesh;
    public Transform Slicer;
    public Pointer pointer;
    public Transform startingPoint;

    float timeBetweenFrames = 1.0f;
    float timeLeft = 1.0f;
    bool paused = false;
	// Use this for initialization
	void Start ()
    {
      
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.gameObject.SetActive(parser.isDynamic);

        if(!paused)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f)
            {
                resetTimer();
            }
        }
        if(slider != null)
        {
            if (slider.value >= slider.maxValue)
            {
                slider.value = slider.minValue;
            }
            parser.currentFrame = (int)slider.value;
        }
	}
    
    void resetTimer()
    {
        if (slider != null)
        {
            timeLeft = timeBetweenFrames;
            slider.value += 1;
        }
    }

    public void pause()
    {
        paused = !paused;
    }

    public void resetMesh()
    {
        GameObject.Find("BrainNodes").transform.rotation = Quaternion.identity;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("BrainMesh"))
        {
            Destroy(go);
        }
        Transform brainChild = (Transform)Instantiate(BrainMesh, new Vector3(0, 0, 0), Quaternion.identity);
        brainChild.parent = GameObject.Find("BrainNodes").transform;
        brainChild.parent.localScale = new Vector3(1, 1, 1);
        brainChild.GetComponent<BrainMeshSplitter>().Slicer = Slicer;
        brainChild.GetComponent<BrainMeshSplitter>().pointer = pointer;
        brainChild.GetComponent<BrainMeshSplitter>().startingPoint = startingPoint;
        brainChild.GetComponent<BrainMeshSlicing>().Slicer = Slicer;
        brainChild.GetComponent<BrainMeshSlicing>().pointer = pointer;
    }
}

