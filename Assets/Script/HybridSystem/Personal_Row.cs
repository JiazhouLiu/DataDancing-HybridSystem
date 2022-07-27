using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personal_Row : MonoBehaviour
{
    [Header("Parent Script")]
    public ViewManager VM;
    public PersonalWorkSpace PWS;
    public Personal_Row baseRow;

    [Header("Variables")]
    public int rowNumber;
    public float rowHeight;
    public bool faceToUser;
    public int numberLimit;

    // parameter
    [HideInInspector]
    public float radius;
    private float perimeter;
    private float adjustedRowHeight;

    // items
    private int currentObjectNumber;
    private int previousObjectNumber = 0;

    private List<GameObject> visList;

    // line renderer
    private LineRenderer lineRenderer;

    private int vertexCount = 0;
    private float rotationOffset = 0;
    private float rowAngleOffset = 0;
    private float currentParentAngleOffset = 0;

    // vector smooth filter
    private OneEuroFilter<Vector3> vector3Filter;
    private float filterFrequency = 60.0f;

    // inheritance
    private Transform User;
    private float ObjectDistance;
    private float ObjectSize;
    private float angleOffset; // need to switch it back to private when foot is enabled
    private float minRadius;
    private int smoothDelta;
    private float animationSpeed;
    private float UserHeightOffset;
    private float WorkSpaceHeight;

    private void Awake()
    {
        User = VM.User;

        visList = new List<GameObject>();

        radius = 0f;
        lineRenderer = GetComponent<LineRenderer>();

        InitiateViews();

        currentObjectNumber = transform.childCount;

        ObjectDistance = PWS.ObjectDistance;
        ObjectSize = PWS.ObjectSize;

        ObjectDistance += ObjectSize;

        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency);
    }

    // Update is called once per frame
    void Update()
    {
        angleOffset = PWS.angleOffset;
        rotationOffset = PWS.rotationOffset;
        minRadius = PWS.minRadius;
        smoothDelta = PWS.smoothDelta;
        animationSpeed = PWS.animationSpeed;
        UserHeightOffset = PWS.UserHeightOffset;
        WorkSpaceHeight = PWS.WorkSpaceHeight;

        if (PWS.rowHeightOffset > 0.05f)
            faceToUser = true;
        else
            faceToUser = false;

        adjustedRowHeight = rowHeight + WorkSpaceHeight + rowNumber * PWS.rowHeightOffset;

        currentObjectNumber = transform.childCount;

        numberLimit = (int) Mathf.Ceil(perimeter/ObjectDistance);

        if (rowNumber != 0) {
            if (previousObjectNumber == 0 && currentObjectNumber != 0) {
                rowAngleOffset = -90 + User.localEulerAngles.y - currentParentAngleOffset;
            }
            
        }


        if (currentObjectNumber != previousObjectNumber)
            InitiateViews();

        if (rowNumber == 0)
        {
            float minPerimeter = 2 * minRadius * Mathf.PI;

            if (minPerimeter > currentObjectNumber * ObjectDistance) // min Circle
            {
                radius = minRadius;
                perimeter = minPerimeter;

                vertexCount = (int)(perimeter / ObjectDistance) + 1;

                int smoothVertexCount = vertexCount * smoothDelta;

                Vector3 userPosition = vector3Filter.Filter(User.position);
                transform.position = userPosition;

                //angleOffset = Vector3.SignedAngle(User.forward, new Vector3(0, User.position.y, 0) - User.position, Vector3.up);
                SetupCircle(radius, smoothVertexCount, angleOffset + rotationOffset);

                foreach (Transform t in transform)
                {
                    t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * adjustedRowHeight, Time.deltaTime * animationSpeed);

                    t.localScale = Vector3.one * ObjectSize;

                    t.LookAt(User.transform.position);
                    if (faceToUser)
                        t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                    else
                        t.localEulerAngles = new Vector3(90, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                }

            }
            else // dynamic large circle
            {
                perimeter = currentObjectNumber * ObjectDistance;
                radius = perimeter / (2 * Mathf.PI);

                Vector3 userPosition = vector3Filter.Filter(User.position);
                transform.position = userPosition - User.forward * (radius - minRadius);

                vertexCount = currentObjectNumber;

                int smoothVertexCount = vertexCount * smoothDelta;

                //angleOffset = Vector3.SignedAngle(User.forward, Vector3.zero - User.position, Vector3.up);
                SetupCircle(radius, smoothVertexCount, angleOffset + rotationOffset);

                foreach (Transform t in transform)
                {

                    t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * adjustedRowHeight, Time.deltaTime * animationSpeed);

                    t.localScale = Vector3.one * ObjectSize;

                    t.LookAt(User.transform.position);
                    if (faceToUser)
                        t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                    else
                        t.localEulerAngles = new Vector3(90, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                }
            }
        }
        else {
            radius = baseRow.radius + ObjectDistance * rowNumber - PWS.rowHeightOffset * rowNumber * 0.5f;
            perimeter = 2 * radius * Mathf.PI;

            Vector3 userPosition = vector3Filter.Filter(User.position);
            transform.position = userPosition - User.forward * (baseRow.radius - minRadius);

            vertexCount = (int)(perimeter / ObjectDistance) + 1;

            int smoothVertexCount = vertexCount * smoothDelta;

            //angleOffset = Vector3.SignedAngle(User.forward, Vector3.zero - User.position, Vector3.up);
            SetupCircle(radius, smoothVertexCount, angleOffset + rotationOffset + rowAngleOffset);

            foreach (Transform t in transform)
            {

                t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * adjustedRowHeight, Time.deltaTime * animationSpeed);

                t.localScale = Vector3.one * ObjectSize;

                t.LookAt(User.transform.position);
                if (faceToUser)
                    t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                else
                    t.localEulerAngles = new Vector3(90, t.localEulerAngles.y, t.localEulerAngles.z + 180);
            }
        }

        

        previousObjectNumber = currentObjectNumber;
        currentParentAngleOffset = angleOffset;
    }

    private void InitiateViews()
    {

        visList.Clear();

        foreach (Transform child in transform)
        {
            visList.Add(child.gameObject);
        }
    }


    private void SetupCircle(float r, int vCount, float angleOffset)
    {
        float deltaTheta = (2f * Mathf.PI) / vCount;
        float theta = -Mathf.Deg2Rad * angleOffset;

        lineRenderer.positionCount = vCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(r * Mathf.Cos(theta) + transform.position.x, 0f, r * Mathf.Sin(theta) + transform.position.z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}
