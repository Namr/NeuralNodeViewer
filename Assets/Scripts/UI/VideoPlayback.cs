﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayback : MonoBehaviour {
    public Slider slider;
    public NodeParser parser;

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
        if(!paused)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f)
            {
                resetTimer();
            }
        }
        if (slider.value >= slider.maxValue)
        {
            slider.value = slider.minValue;
        }
        parser.currentFrame = (int) slider.value;
	}
    
    void resetTimer()
    {
        timeLeft = timeBetweenFrames;
        slider.value += 1;
    }

    public void pause()
    {
        paused = !paused;
    }
}

