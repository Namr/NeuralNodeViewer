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

    Vector3 vectorDistance;
    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update()
    {

        Speed = 10;
        YAngle = Input.GetAxis("Oculus_Touch_RThumbstickX") * Speed * Time.deltaTime;
        XAngle = Input.GetAxis("Oculus_Touch_RThumbstickY") * Speed * Time.deltaTime;
       // YAngle = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch).eulerAngles;
        transform.Rotate(new Vector3(0.0f, 0.0f, YAngle));
        transform.Rotate(new Vector3(XAngle, 0.0f, 0.0f));
        if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            transform.Rotate(OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch).y * 2, OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch).z * 2, OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch).x * 2);
        }
        /*
        if (Input.GetButtonUp("Fire1"))
        {
            transform.position = startPos;
            transform.rotation = startRot;
        }
        */
    }

    public void snapToAngle(int dir)
    {
        switch (dir)
        {
            case 0:
                transform.eulerAngles = new Vector3(0,0,0);
                break;
            case 1:
                transform.eulerAngles = new Vector3(-90, 90, 0);
                break;
            case 2:
                transform.eulerAngles = new Vector3(-90, -90, 0);
                break;
            case 3:
                transform.eulerAngles = new Vector3(-90, 360, 0);
                break;
            case 4:
                transform.eulerAngles = new Vector3(-180, 360, 0);
                break;
        }   
    }
}
