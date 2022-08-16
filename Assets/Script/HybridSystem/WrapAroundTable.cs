using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Surface { 
    TableCircle,
    BodyCircle,
    Wall
}

[RequireComponent(typeof(LineRenderer))]
public class WrapAroundTable : MonoBehaviour
{
    [Header("Reference")]
    public Transform User;
    public Transform Waist;
    public GameObject ObjectPrefab;
    public BoxCollider thisCollider;

    [Header("Variables")]
    public float ObjectSize = 0.5f;
    public int ObjectNumber = 6;
    public float fakeUserHeight = 1.8f;
    public float fakeWaistHeight = 1f;
    public float minRadius = 0.7f;
    public int smoothDelta = 10;
    public float animationSpeed = 10;
    public bool EXP = true;
    public bool smoothToCircle = false;
    public bool faceToUser = false;
    public Surface currentSurface;

    private float radius;
    private float perimeter;
    private float angleOffset;

    private LineRenderer lineRenderer;

    private int vertexCount = 0;
    private float previousAngleOffset = 0;

    private OneEuroFilter<Vector3> vector3Filter;
    private float filterFrequency = 60.0f;

    private List<GameObject> visList;

    private float tableHeight = 0;
    private float wallHeight = 0;

    // Start is called before the first frame update
    void Awake()
    {
        visList = new List<GameObject>();

        if (Waist.position != Vector3.zero)
            tableHeight = Waist.position.y;
        else
            tableHeight = fakeWaistHeight;

        if (User.position.y == 0)
            wallHeight = fakeUserHeight;
        else
            wallHeight = User.position.y - 0.2f;

        transform.position = new Vector3(0, wallHeight, 0);

        radius = 0f;
        lineRenderer = GetComponent<LineRenderer>();

        List<Vector3> objectPositions = new List<Vector3>();
        for (int i = 0; i < ObjectNumber; i++)
        {
            GameObject go = Instantiate(ObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            go.name = "object" + (i + 1);
            go.transform.SetParent(transform);

            Vis_WrapAround vis = new Vis_WrapAround() {
                VisName = go.name,
                WallPosition = GetAvaiableRandomPosition(objectPositions, 0.3f)
            };

            go.GetComponent<Vis_WrapAround>().CopyEntity(vis);
            go.GetComponent<Vis_WrapAround>().OnWall = true;
            go.GetComponent<Vis_WrapAround>().TableCirclePosition = go.GetComponent<Vis_WrapAround>().ConvertPositions(vis.WallPosition, "Wall");

            objectPositions.Add(go.GetComponent<Vis_WrapAround>().WallPosition);
            go.transform.localPosition = go.GetComponent<Vis_WrapAround>().WallPosition;
            visList.Add(go);
        }

        vector3Filter = new OneEuroFilter<Vector3>(filterFrequency);
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserPosition();

        if (currentSurface == Surface.Wall)
        {
            thisCollider.size = new Vector3(4, 1.2f, 0.1f);
            thisCollider.center = Vector3.zero;
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, wallHeight, 0), Time.deltaTime * animationSpeed);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, Time.deltaTime * animationSpeed);

            foreach (GameObject go in visList)
            {
                go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, go.GetComponent<Vis_WrapAround>().WallPosition,
                    Time.deltaTime * animationSpeed);
                go.transform.localEulerAngles = Vector3.zero;
            }
        }
        else if (currentSurface == Surface.TableCircle) {

            thisCollider.size = new Vector3(radius*2, radius + 0.4f, 0.1f);
            thisCollider.center = new Vector3(0, 0.2f - radius / 2, 0);
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, tableHeight, 0), Time.deltaTime * animationSpeed);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(90, 0, 0), Time.deltaTime * animationSpeed);

            foreach (GameObject go in visList)
            {
                go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, go.GetComponent<Vis_WrapAround>().TableCirclePosition,
                    Time.deltaTime * animationSpeed);

                go.transform.LookAt(User.transform.position);
                go.transform.localEulerAngles = new Vector3(0, 0, go.transform.localEulerAngles.z + 180);
            }
            
        }
        else if (currentSurface == Surface.BodyCircle)
        {
            thisCollider.size = new Vector3(minRadius*2, minRadius*2, 0.1f);
            thisCollider.center = Vector3.zero;
            transform.position = Vector3.Lerp(transform.position, new Vector3(User.position.x, tableHeight, User.position.z), Time.deltaTime * animationSpeed);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(90, 0, 0), Time.deltaTime * animationSpeed);

            foreach (GameObject go in visList)
            {
                go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, go.GetComponent<Vis_WrapAround>().BodyCirclePosition,
                    Time.deltaTime * animationSpeed);

                go.transform.LookAt(User.transform.position);
                go.transform.localEulerAngles = new Vector3(0, 0, go.transform.localEulerAngles.z + 180);
            }
        }

        previousAngleOffset = angleOffset;
    }


    private void CheckUserPosition() {
        if (User.position.z < -minRadius * 2) { // wall display
            RemoveCircle();
            currentSurface = Surface.Wall;
            foreach (GameObject go in visList)
            {
                go.transform.GetComponent<Vis_WrapAround>().OnWall = true;
                go.transform.GetComponent<Vis_WrapAround>().OnTable = false;
            }
        }
        else if (User.position.z < -minRadius) //  large circle table display
        {
            currentSurface = Surface.TableCircle;

            // calculate radius and perimeter
            radius = Mathf.Abs(User.position.z);
            perimeter = radius * 2 * Mathf.PI;
            vertexCount = (int)((radius - minRadius) / minRadius * 50 + 50);
            int smoothVertexCount = vertexCount * smoothDelta;
            angleOffset = Vector3.SignedAngle(User.forward, new Vector3(0, User.position.y, 0) - User.position, Vector3.up) * 3f;

            SetupCircle(radius, smoothVertexCount, angleOffset);

            foreach (GameObject go in visList)
            {
                go.transform.GetComponent<Vis_WrapAround>().OnBody = false;
                go.transform.GetComponent<Vis_WrapAround>().OnTable = true;
                go.transform.GetComponent<Vis_WrapAround>().OnWall = false;

                float objectWallX = go.GetComponent<Vis_WrapAround>().WallPosition.x;
                int index = (int)((2 - objectWallX) / 4 * 500);
                Vector3 linePosition = User.InverseTransformPoint(lineRenderer.GetPosition(index));
                
                float objectWallY = go.GetComponent<Vis_WrapAround>().WallPosition.y;
                float newX = linePosition.x / radius * (radius / ((objectWallY - 1.35f) / -0.9f));
                float newZ = linePosition.z / radius * (radius / ((objectWallY - 1.35f) / -0.9f));

                go.GetComponent<Vis_WrapAround>().TableCirclePosition = transform.InverseTransformPoint(User.TransformPoint(new Vector3(newX, linePosition.y, newZ)));
            }
        }
        else { // follow body circle
            currentSurface = Surface.BodyCircle;

            radius = minRadius;
            perimeter = radius * 2 * Mathf.PI;
            vertexCount = 50;
            int smoothVertexCount = vertexCount * smoothDelta;
            angleOffset = previousAngleOffset;

            SetupCircle(radius, smoothVertexCount, angleOffset);

            foreach (GameObject go in visList)
            {
                go.transform.GetComponent<Vis_WrapAround>().OnBody = true;
                go.transform.GetComponent<Vis_WrapAround>().OnTable = false;
                go.transform.GetComponent<Vis_WrapAround>().OnWall = false;

                float objectWallX = go.GetComponent<Vis_WrapAround>().WallPosition.x;
                int index = (int)((2 - objectWallX) / 4 * 500);
                Vector3 linePosition = User.InverseTransformPoint(lineRenderer.GetPosition(index));

                float objectWallY = go.GetComponent<Vis_WrapAround>().WallPosition.y;
                float newX = linePosition.x / radius * (radius / ((objectWallY - 1.35f) / -0.9f));
                float newZ = linePosition.z / radius * (radius / ((objectWallY - 1.35f) / -0.9f));

                go.GetComponent<Vis_WrapAround>().BodyCirclePosition = transform.InverseTransformPoint(User.TransformPoint(new Vector3(newX, linePosition.y, newZ)));
            }
        }
    }

    private Vector3 GetAvaiableRandomPosition(List<Vector3> currentList, float size)
    {
        Vector3 tmpPosition = Vector3.zero;

        tmpPosition = new Vector3(Random.Range(-1.85f, 1.85f), Random.Range(-0.45f, 0.45f), 0);
        

        if (currentList.Count > 0)
        {
            foreach (Vector3 v in currentList)
            {
                if (Vector3.Distance(v, tmpPosition) < size * 1.1f)
                {
                    tmpPosition = GetAvaiableRandomPosition(currentList, size);
                }
            }
        }

        return tmpPosition;
    }

    private void SetupCircle(float r, int vCount, float angleOffset)
    {
        float deltaTheta = (2f * Mathf.PI) / vCount;
        float theta = -Mathf.Deg2Rad * angleOffset;
        Vector3 userPosition = vector3Filter.Filter(User.position);

        lineRenderer.positionCount = vCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(r * Mathf.Cos(theta) + User.position.x, tableHeight, r * Mathf.Sin(theta) + User.position.z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    private void RemoveCircle() {
        lineRenderer.positionCount = 0;
    }
}
