﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootGestureDetector : MonoBehaviour
{
    [Header("Two Feet Prefabs")]
    // left foot
    public Transform leftFoot;
    public Transform leftFootToe;
    public Transform leftFootHeel;
    //public FootToeCollision leftFootToeCollision;
    public ShoeRecieve leftSR;
    // right foot
    public Transform rightFoot;
    public Transform rightFootToe;
    public Transform rightFootHeel;
    //public FootToeCollision rightFootToeCollision;
    public ShoeRecieve rightSR;

    [Header("Pressure Sensor Variables")]
    public int pressToSelectThresholdLeft = 0;
    public int holdThresholdLeft = 500;
    public int releaseThresholdLeft = 1000;
    public int pressToSelectThresholdRight = 0;
    public int holdThresholdRight = 500;
    public int releaseThresholdRight = 1000;

    [Header("Sliding Gesture")]
    public bool footSlideToRight = false;
    public bool footSlideToLeft = false;
    public bool footSlideToFront = false;
    public bool footSlideToBack = false;

    [Header("Rotation Gesture")]
    public bool footRotateToRight = false;
    public bool footRotateToLeft = false;


    // pressure sensor
    [HideInInspector] public bool leftNormalPressFlag = false;
    [HideInInspector] public bool rightNormalPressFlag = false;
    [HideInInspector] public bool leftHoldingFlag = false;
    [HideInInspector] public bool rightHoldingFlag = false;

    private Vector3 previousLeftPosition;
    private Vector3 previousLeftToePosition;
    private Vector3 previousLeftHeelPosition;
    [HideInInspector] public bool leftMoving = false;
    private Vector3 previousRightPosition;
    private Vector3 previousRightToePosition;
    private Vector3 previousRightHeelPosition;
    [HideInInspector] public bool rightMoving = false;

    private float leftTotalDistance = 0;
    private float leftToeTotalDistance = 0;
    private float leftHeelTotalDistance = 0;
    private float rightTotalDistance = 0;
    private float rightToeTotalDistance = 0;
    private float rightHeelTotalDistance = 0;

    // Start is called before the first frame update
    void Start()
    {
        previousLeftPosition = leftFoot.position;
        previousLeftToePosition = leftFootToe.position;
        previousLeftHeelPosition = leftFootHeel.position;
        previousRightPosition = rightFoot.position;
        previousRightToePosition = rightFootToe.position;
        previousRightHeelPosition = rightFootHeel.position;
    }

    // Update is called once per frame
    void Update()
    {
        PressureSensorDetector();
        previousLeftPosition = leftFoot.position;
        previousLeftToePosition = leftFootToe.position;
        previousLeftHeelPosition = leftFootHeel.position;
        previousRightPosition = rightFoot.position;
        previousRightToePosition = rightFootToe.position;
        previousRightHeelPosition = rightFootHeel.position;
    }

    private void PressureSensorDetector()
    {
        // Press Detect - Left
        //if (leftSR.value.Length > 0 && int.Parse(leftSR.value) <= pressToSelectThresholdLeft && !leftNormalPressFlag)
        //    leftNormalPressFlag = true;
        //if (leftNormalPressFlag && leftSR.value.Length > 0 && int.Parse(leftSR.value) > releaseThresholdLeft)
        //{
        //    leftNormalPressFlag = false;
        //    if (leftTotalDistance < 0.1f && rightTotalDistance < 0.1f)
        //    {
        //        Debug.Log("left foot press");
        //    }
        //}

        //// Press Detect - Right
        //if (rightSR.value.Length > 0 && int.Parse(rightSR.value) <= pressToSelectThresholdRight && !rightNormalPressFlag)
        //    rightNormalPressFlag = true;
        //if (rightNormalPressFlag && rightSR.value.Length > 0 && int.Parse(rightSR.value) > releaseThresholdRight)
        //{
        //    rightNormalPressFlag = false;
        //    if (leftTotalDistance < 0.1f && rightTotalDistance < 0.1f)
        //    {
        //        Debug.Log("right foot press");
        //    }
        //}

        // Sliding Detect - Left
        if (leftSR.value.Length > 0)
        {
            if (int.Parse(leftSR.value) < holdThresholdLeft)
                leftHoldingFlag = true;
            else
                leftHoldingFlag = false;
        }

        // Sliding Detect - Right
        if (rightSR.value.Length > 0)
        {
            if (int.Parse(rightSR.value) < holdThresholdRight)
                rightHoldingFlag = true;
            else
                rightHoldingFlag = false;
        }

        if (Vector3.Distance(leftFoot.position, previousLeftPosition) > 0.005f && leftHoldingFlag) // left moving
            leftMoving = true;
        else if (Vector3.Distance(leftFoot.position, previousLeftPosition) <= 0.005f && leftSR.value.Length > 0 && int.Parse(leftSR.value) > releaseThresholdLeft) // left still
            leftMoving = false;

        if (leftFoot.position.y > 0.1f)
            leftMoving = false;

        if (Vector3.Distance(rightFoot.position, previousRightPosition) > 0.005f && rightHoldingFlag) // right moving
            rightMoving = true;
        else if (Vector3.Distance(rightFoot.position, previousRightPosition) <= 0.005f && rightSR.value.Length > 0 && int.Parse(rightSR.value) > releaseThresholdRight) // right still
            rightMoving = false;

        if (rightFoot.position.y > 0.1f)
            rightMoving = false;

        if (leftMoving)
        {
            leftTotalDistance += Vector3.Distance(leftFoot.position, previousLeftPosition);
            leftToeTotalDistance += Vector3.Distance(leftFootToe.position, previousLeftToePosition);
            leftHeelTotalDistance += Vector3.Distance(leftFootHeel.position, previousLeftHeelPosition);
            Debug.Log("Left Moving");
        }
        else {
            leftTotalDistance = 0;
            leftToeTotalDistance = 0;
            leftHeelTotalDistance = 0;
        }


        if (rightMoving) {
            rightTotalDistance += Vector3.Distance(rightFoot.position, previousRightPosition);
            rightToeTotalDistance += Vector3.Distance(rightFootToe.position, previousRightToePosition);
            rightHeelTotalDistance += Vector3.Distance(rightFootHeel.position, previousRightHeelPosition);
            Debug.Log("Right Moving");
        }
        else {
            rightTotalDistance = 0;
            rightToeTotalDistance = 0;
            rightHeelTotalDistance = 0;
        }

        Debug.Log("Left Foot Distance: " + leftTotalDistance + "; Left Toe Distance: " + leftToeTotalDistance + "; Left Heel Distance: " + leftHeelTotalDistance);
        Debug.Log("Right Foot Distance: " + rightTotalDistance + "; Right Toe Distance: " + rightToeTotalDistance + "; Right Heel Distance: " + rightHeelTotalDistance);
    }
}