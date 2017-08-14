using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainMovement : MonoBehaviour {
    float XAngle;
    float YAngle;
    float ZAngle;

    float forwardMovement;
    float sideMovement;

    float Speed = 10;

    Vector3 startPos;
    Vector3 vectorDistance;
    Quaternion startRot;
    // Use this for initialization
    void Start ()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {

        Speed = 10;
        YAngle = Input.GetAxis("Oculus_Touch_RThumbstickX") * Speed * Time.deltaTime;
        XAngle = Input.GetAxis("Oculus_Touch_RThumbstickY") * Speed * Time.deltaTime;

        transform.Rotate(new Vector3(0.0f, 0.0f, YAngle));
        transform.Rotate(new Vector3(XAngle,0.0f, 0.0f));

        if (Input.GetButtonUp("Fire1"))
        {
            transform.position = startPos;
            transform.rotation = startRot;
        }
    }
}
