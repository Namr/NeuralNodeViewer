using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public enum State
    {
        EMPTY,
        TOUCHING,
        HOLDING
    };

    public OVRInput.Controller Controller = OVRInput.Controller.LTouch;
    public State mHandState = State.EMPTY;
    public Rigidbody AttachPoint = null;
    public bool IgnoreContactPoint = false;
    public string tag = "Grabbable";
    private Transform mHeldObject;
    private FixedJoint mTempJoint;
    private Transform prevTransfrom;

    void Start()
    {
        if (AttachPoint == null)
        {
            AttachPoint = GetComponent<Rigidbody>();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (mHandState == State.EMPTY)
        {
            GameObject temp = collider.gameObject;
            if (temp != null && temp.tag == tag && temp.GetComponent<Transform>() != null)
            {
                mHeldObject = temp.GetComponent<Transform>();
                prevTransfrom = mHeldObject.parent;
                mHandState = State.TOUCHING;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (mHandState != State.HOLDING)
        {
            if (collider.gameObject.tag == tag)
            {
                mHeldObject = null;
                mHandState = State.EMPTY;
            }
        }
    }

    void Update()
    {
        switch (mHandState)
        {
            case State.TOUCHING:
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) >= 0.5f)
                {
                    mHeldObject.parent = transform;
                    mHandState = State.HOLDING;
                }
                break;
            case State.HOLDING:
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) < 0.5f)
                {
                    if(prevTransfrom != null)
                    {
                        mHeldObject.parent = prevTransfrom;
                    }
                    else
                    {
                        mHeldObject.parent = null;
                    }
                    mHandState = State.EMPTY;
                }
                break;
        }
    }

    private void throwObject()
    {
    }
}
