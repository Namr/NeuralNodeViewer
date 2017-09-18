using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour {

    float XAngle;
    float YAngle;
    float ZAngle;

    float forwardMovement;
    float sideMovement;

    float Speed = 10;

    Vector3 vectorDistance;
	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        
        Speed = 10f;

   
        sideMovement = Input.GetAxis("Oculus_Touch_LThumbstickX") * Speed * Time.deltaTime;
        forwardMovement = Input.GetAxis("Oculus_Touch_LThumbstickY") * Speed * Time.deltaTime;

        transform.Translate(new Vector3(sideMovement, 0.0f, forwardMovement));

    }
}
