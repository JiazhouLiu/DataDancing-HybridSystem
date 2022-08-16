using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WrapAround : MonoBehaviour
{
    [Header("Reference")]
    public Transform User;
    public GameObject ObjectPrefab;

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
    public bool faceToUser = true;

    private float radius;
    private float perimeter;
    private float angleOffset;

    private LineRenderer lineRenderer;

    private int vertexCount = 0;
    private float previousAngleOffset = 0;

    private OneEuroFilter<Vector3> vector3Filter;
    private float filterFrequency = 60.0f;

    private Dictionary<Transform, float> heightList;
    private List<GameObject> visList;

    // Start is called before the first frame update
    private void Awake()
    {
        heightList = new Dictionary<Transform, float>();
        visList = new List<GameObject>();

        radius = 0f;
        lineRenderer = GetComponent<LineRenderer>();
        for (int i = 0; i < ObjectNumber; i++)
        {
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
        if (User.position.z < -2) {
            radius = 1000;
            perimeter = 2 * radius * Mathf.PI;

            vertexCount = (int)(perimeter / ObjectDistance) + 1;
            int smoothVertexCount = vertexCount * smoothDelta;
            angleOffset = previousAngleOffset;

            SetupCircle(radius, smoothVertexCount, (angleOffset));

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
                if (faceToUser)
                    t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                else
                    t.localEulerAngles = new Vector3(90, t.localEulerAngles.y, t.localEulerAngles.z + 180);
            }


        }
        else if (User.position.z < -minRadius)
        {
            // calculate radius and perimeter
            radius = Mathf.Abs(User.position.z);

            if (EXP)
                radius = Mathf.Exp(-(User.position.z + minRadius) / 5) * Mathf.Abs(User.position.z);

            if (radius < minRadius)
                radius = minRadius;

            ///Debug.Log(radius);

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
                        if (faceToUser)
                            t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                        else
                            t.localEulerAngles = new Vector3(90, t.localEulerAngles.y, t.localEulerAngles.z + 180);
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

                            t.LookAt(User.transform.position + Vector3.up * UserHeightOffset);
                            if (faceToUser)
                                t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                            else
                                t.localEulerAngles = new Vector3(90, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                        }
                    }
                    else
                    {
                        radius = minRadius;

                        perimeter = ObjectNumber * ObjectDistance;
                        vertexCount = ObjectNumber;
                        smoothVertexCount = vertexCount * smoothDelta;
                        angleOffset = Vector3.SignedAngle(User.forward, new Vector3(0, User.position.y, 0) - User.position, Vector3.up) * 3f;

                        SetupCircle(radius, smoothVertexCount, angleOffset);

                        foreach (Transform t in transform)
                        {

                            t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t], Time.deltaTime * animationSpeed);

                            t.LookAt(User.transform.position + Vector3.up * UserHeightOffset);
                            if (faceToUser)
                                t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                            else
                                t.localEulerAngles = new Vector3(90, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                        }
                    }
                }

            }
            else
            {
                perimeter = ObjectNumber * ObjectDistance;
                vertexCount = ObjectNumber;
                int smoothVertexCount = vertexCount * smoothDelta;
                angleOffset = previousAngleOffset;

                SetupCircle(radius, smoothVertexCount, (angleOffset));

                foreach (Transform t in transform)
                {
                    t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t], Time.deltaTime * animationSpeed);

                    t.LookAt(User.transform.position + Vector3.up * UserHeightOffset);
                    if (faceToUser)
                        t.localEulerAngles = new Vector3(t.localEulerAngles.x + 180, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                    else
                        t.localEulerAngles = new Vector3(90, t.localEulerAngles.y, t.localEulerAngles.z + 180);
                }
            }
        }
        else
        {
            radius = minRadius;

            perimeter = ObjectNumber * ObjectDistance;
            vertexCount = ObjectNumber;
            int smoothVertexCount = vertexCount * smoothDelta;
            angleOffset = previousAngleOffset;

            SetupCircle(radius, smoothVertexCount, (angleOffset));

            foreach (Transform t in transform)
            {

                t.position = Vector3.Lerp(t.position, lineRenderer.GetPosition(t.GetSiblingIndex() * smoothDelta) + Vector3.up * heightList[t], Time.deltaTime * animationSpeed);

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

        if (EXP && User.position.z < 0 && r > minRadius)
        {

            Vector3 normV = new Vector3(0, 0, userPosition.z).normalized * r + new Vector3(userPosition.x, 0, 0);
            transform.position = normV;
        }
        else
            transform.position = userPosition;

        lineRenderer.positionCount = vCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(r * Mathf.Cos(theta) + transform.position.x, 0f, r * Mathf.Sin(theta) + transform.position.z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}
