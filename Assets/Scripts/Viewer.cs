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

    public Transform Fulcrum;
    public Transform Brain;

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
        vectorDistance = transform.position - Brain.position;
        Speed = vectorDistance.magnitude * 0.2f;
        YAngle = Input.GetAxis("Oculus_Touch_RThumbstickX") * Speed * Time.deltaTime;
        XAngle = Input.GetAxis("Oculus_Touch_RThumbstickY") * Speed * Time.deltaTime;

        
        if(Input.GetButton("Fire2"))
        {
            sideMovement = Input.GetAxis("Oculus_Touch_LThumbstickX") * Speed * Time.deltaTime;
        }
        else
        {
            ZAngle = Input.GetAxis("Oculus_Touch_LThumbstickX") * Speed * Time.deltaTime;
            sideMovement = 0;
        }

        forwardMovement = Input.GetAxis("Oculus_Touch_LThumbstickY") * Speed * Time.deltaTime;

        transform.RotateAround(Fulcrum.position,new Vector3(1.0f,0.0f,0.0f),XAngle);
        transform.RotateAround(Fulcrum.position, new Vector3(0.0f, 1.0f, 0.0f), YAngle);

        transform.Rotate(new Vector3(0, 0, ZAngle / Speed * 20));
        transform.Translate(new Vector3(sideMovement, 0.0f, forwardMovement));

        if(Input.GetButtonUp("Fire1"))
        {
            XAngle = 0;
            YAngle = 0;
            transform.position = startPos;
            transform.rotation = startRot;
        }
    }
}
