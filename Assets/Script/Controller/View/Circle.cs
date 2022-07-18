﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Circle : MonoBehaviour
{
    [Header("Reference")]
    public Transform User;
    public GameObject ObjectPrefab;
    public GameObject IntersectionPrefab;

    [Header("Variables")]
    public float ObjectSize = 0.5f;
    public float ObjectDistance = 0.1f;
    public int ObjectNumber = 6;
    public float UserHeightOffset = 1.8f;
    public float minRadius = 0.7f;
    public int smoothDelta = 10;
    public float animationSpeed = 10;
    public bool EXP = true;
    public bool smoothToCircle = false;

    private float radius;
    private float perimeter;
    private float angleOffset;

    private LineRenderer lineRenderer;

    private int vertexCount = 0;
    private float previousAngleOffset = 0;

    private OneEuroFilter<Vector3> vector3Filter;
    private float filterFrequency = 60.0f;

    private bool closePointFlag = false;

    private Dictionary<Transform, float> heightList;
    private List<GameObject> visList;

    private int currentClosestIndex = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        heightList = new Dictionary<Transform, float>();
        visList = new List<GameObject>();

        radius = 0f;
        lineRenderer = GetComponent<LineRenderer>();
        for (int i = 0; i < ObjectNumber; i++) {
            GameObject go = Instantiate(ObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            go.name = "object" + i;
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one * ObjectSize;

            heightList.Add(go.transform, 1.3f);
            visList.Add(go);
        }
        ObjectDistance += ObjectSize;

        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency);
    }

    private void Update()
    {
        GameObject closestObj = null;
        Vector3 userToObj = Vector3.zero;
        //currentClosestIndex = FindIntersectionIndex(User, lineRenderer);
        if (User.position.z < -minRadius)
        {
            // closest object indicator
            foreach (GameObject go in visList) {
                foreach (Transform t in go.GetComponent<Vis>().VisBorder) { 
                    t.GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }
            closestObj = FindNearestObj(User, lineRenderer);
            userToObj = closestObj.transform.position - User.position;
            //foreach (Transform t in closestObj.GetComponent<Vis>().VisBorder)
            //{
            //    t.GetComponent<MeshRenderer>().material.color = Color.red;
            //}

            // calculate radius and perimeter
            radius = Mathf.Abs(User.position.z);

            if (EXP)
                radius = Mathf.Exp(-(User.position.z + minRadius) / 5) * Mathf.Abs(User.position.z);

            if (radius < minRadius)
                radius = minRadius;

            perimeter = 2 * radius * Mathf.PI;

            if (perimeter > ObjectNumber * ObjectDistance)
            {
                vertexCount = (int)(perimeter / ObjectDistance) + 1;

                int smoothVertexCount = vertexCount * smoothDelta;

                if (vertexCount > ObjectNumber * 2 && smoothVertexCount / 4 - (3 * smoothDelta) > 0)
                {
                    angleOffset = Vector3.SignedAngle(User.forward, new Vector3(0, User.position.y, 0) - User.position, Vector3.up);
                    SetupCircle(radius, smoothVertexCount, angleOffset);

                    int j = smoothVertexCount / 4 - (3 * smoothDelta);

                    foreach (Transform t in transform)
                    {
                        t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(j) + Vector3.up * heightList[t], Time.deltaTime * animationSpeed);
                        //t.position = lineRenderer.GetPosition(j) + Vector3.up * heightList[t];
                        //if(t.position.z > 0)
                        //    t.position = new Vector3(t.position.x, t.position.y, 0);
                        j = j + smoothDelta;

                        t.LookAt(User.transform.position + Vector3.up * UserHeightOffset);
                        t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                    }

                    
                }
                else
                {
                    if (smoothToCircle)
                    {
                        angleOffset = Vector3.SignedAngle(User.forward, new Vector3(0, User.position.y, 0) - User.position, Vector3.up) * 3f;
                        SetupCircle(radius, smoothVertexCount, angleOffset);

                        foreach (Transform t in transform)
                        {
                            t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t], Time.deltaTime * animationSpeed);
                            //t.position = lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t];
                            //if (t.position.z > 0)
                            //    t.position = new Vector3(t.position.x, t.position.y, 0);
                            t.LookAt(User.transform.position + Vector3.up * UserHeightOffset);
                            t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                        }
                    }
                    else {
                        if (!closePointFlag)
                        {
                            closePointFlag = true;

                        }
                        radius = minRadius;

                        perimeter = ObjectNumber * ObjectDistance;
                        vertexCount = ObjectNumber;
                        smoothVertexCount = vertexCount * smoothDelta;
                        angleOffset = Vector3.SignedAngle(User.forward, new Vector3(0, User.position.y, 0) - User.position, Vector3.up) * 3f;

                        SetupCircle(radius, smoothVertexCount,angleOffset);

                        foreach (Transform t in transform)
                        {

                            t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t], Time.deltaTime * animationSpeed);
                            //t.position = lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t];
                            //if (t.position.z > 0)
                            //    t.position = new Vector3(t.position.x, t.position.y, 0);
                            t.LookAt(User.transform.position + Vector3.up * UserHeightOffset);
                            t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                        }
                    }
                }

            }
            else {
                perimeter = ObjectNumber * ObjectDistance;
                vertexCount = ObjectNumber;
                int smoothVertexCount = vertexCount * smoothDelta;
                //angleOffset = Vector3.SignedAngle(User.forward, Vector3.zero - User.position, Vector3.up);
                angleOffset = previousAngleOffset;
                //Debug.Log("angle offset: " + angleOffset);

                SetupCircle(radius, smoothVertexCount, (angleOffset));

                foreach (Transform t in transform)
                {
                    t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t], Time.deltaTime * animationSpeed);
                    //t.position = lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t];

                    //if (t.position.z > 0)
                    //    t.position = new Vector3(t.position.x, t.position.y, 0);
                    t.LookAt(User.transform.position + Vector3.up * UserHeightOffset);
                    t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                }
            }
        }
        else{
            if (!closePointFlag) {
                closePointFlag = true;
                
            }
            radius = minRadius;

            perimeter = ObjectNumber * ObjectDistance;
            vertexCount = ObjectNumber;
            int smoothVertexCount = vertexCount * smoothDelta;
            //angleOffset = Vector3.SignedAngle(User.forward, Vector3.zero - User.position, Vector3.up);
            angleOffset = previousAngleOffset;
            //Debug.Log("angle offset: " + angleOffset);

            SetupCircle(radius, smoothVertexCount, (angleOffset));

            foreach (Transform t in transform)
            {

                t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t], Time.deltaTime * animationSpeed);
                //t.position = lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t];
                //if (t.position.z > 0)
                //    t.position = new Vector3(t.position.x, t.position.y, 0);
                t.LookAt(User.transform.position + Vector3.up * UserHeightOffset);
                t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
            }
        }

        previousAngleOffset = angleOffset;
    }

    private void SetupCircle(float r, int vCount, float angleOffset)
    {
        float deltaTheta = (2f * Mathf.PI) / vCount;
        float theta = -Mathf.Deg2Rad * angleOffset;
        Vector3 userPosition = vector3Filter.Filter(User.position);

        if (EXP && User.position.z < 0 && r > minRadius) {

            Vector3 normV = new Vector3(0, 0, userPosition.z).normalized * r + new Vector3(userPosition.x, 0, 0);
            transform.position = normV;
        }else
            transform.position = userPosition;

        lineRenderer.positionCount = vCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(r * Mathf.Cos(theta) + transform.position.x, 0f, r * Mathf.Sin(theta) + transform.position.z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    private int FindIntersectionIndex(Transform user, LineRenderer lr) {
        Vector3[] linePositions = new Vector3[lr.positionCount];
        lr.GetPositions(linePositions);

        float maxDist = 10000;
        Vector3 intersectionPoint = Vector3.zero;
        Vector3 userPosition2D = new Vector3(user.position.x, 0, user.position.z);
        int intersectionIndex = 0;
        int i = 0;
        foreach (Vector3 v in linePositions) {
            
            if (Vector3.Distance(userPosition2D, v) < maxDist) {
                maxDist = Vector3.Distance(userPosition2D, v);
                intersectionPoint = v;
                intersectionIndex = i;
            }
            i++;
        }

        return intersectionIndex;
    }

    private GameObject FindNearestObj(Transform user, LineRenderer lr) {

        float maxDist = 10000;
        GameObject nearestGO = null;

        Vector3 userPosition2D = new Vector3(user.position.x, 0, user.position.z);
        

        foreach (GameObject go in visList) {
            Vector3 visPosition2D = new Vector3(go.transform.position.x, 0, go.transform.position.z);
            if (Vector3.Distance(userPosition2D, visPosition2D) < maxDist)
            {
                maxDist = Vector3.Distance(userPosition2D, visPosition2D);
                nearestGO = go;
            }
        }

        return nearestGO;
    }

    //public Vector3 GetPositionFromCircle(Vector3 v) {
    //    float n = (1.57f - v.x) / 3.14f * 500;
    //    Vector3 pos = lineRenderer.GetPosition((int) n);
    //    currentIndex = (int) n;
    //    return new Vector3(pos.x, v.y, pos.z);
    //}
}
