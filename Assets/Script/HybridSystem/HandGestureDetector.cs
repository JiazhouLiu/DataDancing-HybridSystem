using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class HandGestureDetector : MonoBehaviour
{
    [Header("Hand Prefabs")]
    public Transform LeftController;
    public Transform RightController;
    public VRTK_ControllerEvents leftCE;
    public VRTK_ControllerEvents rightCE;

    [Header("Variable")]
    public float controllerMoveThreshold = 0.5f;


    [Header("Observer")]
    public bool leftGrabButtonOn = false;
    public bool rightGrabButtonOn = false;
    public bool expand = false;
    public bool collapse = false;
    public bool moveLeft = false;
    public bool moveRight = false;


    private Vector3 previousLeftControllerPosition;
    private Vector3 previousRightControllerPosition;

    private bool leftToLeft = false;
    private bool leftToRight = false;
    private bool rightToLeft = false;
    private bool rightToRight = false;

    // Update is called once per frame
    void Update()
    {
        if (leftCE.gripPressed && leftGrabButtonOn == false)
            leftGrabButtonOn = true;
        if(!leftCE.gripPressed && leftGrabButtonOn == true)
            leftGrabButtonOn = false;

        if (rightCE.gripPressed && rightGrabButtonOn == false)
            rightGrabButtonOn = true;
        if (!rightCE.gripPressed && rightGrabButtonOn == true)
            rightGrabButtonOn = false;

        CheckHandEvents();

        if (leftToLeft && rightToLeft)
        {
            moveLeft = true;
            expand = false;
            collapse = false;
            moveRight = false;
        }
        else if (leftToLeft && rightToRight)
        {
            expand = true;
            collapse = false;
            moveLeft = false;
            moveRight = false;
        }
        else if (leftToRight && rightToLeft)
        {
            collapse = true;
            expand = false;
            moveLeft = false;
            moveRight = false;
        }
        else if (leftToRight && rightToRight) {
            moveRight = true;
            expand = false;
            collapse = false;
            moveLeft = false;
        }
    }

    private void CheckHandEvents() {
        if (leftGrabButtonOn && rightGrabButtonOn)
        {
            if (LeftController.InverseTransformDirection(LeftController.position - previousLeftControllerPosition).x > controllerMoveThreshold) {
                leftToRight = true;
                leftToLeft = false;
            } else if (LeftController.InverseTransformDirection(LeftController.position - previousLeftControllerPosition).x < -controllerMoveThreshold) {
                leftToLeft = true;
                leftToRight = false;
            }

            if (RightController.InverseTransformDirection(RightController.position - previousRightControllerPosition).x > controllerMoveThreshold)
            {
                rightToRight = true;
                rightToLeft = false;
            }
            else if (RightController.InverseTransformDirection(RightController.position - previousRightControllerPosition).x < -controllerMoveThreshold)
            {
                rightToLeft = true;
                rightToRight = false;
            }
            //Debug.Log(LeftController.InverseTransformDirection(LeftController.position - previousLeftControllerPosition));
        }
        else
        {
            leftToLeft = false;
            leftToRight = false;
            rightToLeft = false;
            rightToRight = false;

            expand = false;
            collapse = false;
            moveLeft = false;
            moveRight = false;

            previousLeftControllerPosition = LeftController.position;
            previousRightControllerPosition = RightController.position;
        }
    }
}
