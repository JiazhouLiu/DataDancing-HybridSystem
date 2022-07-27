using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(LineRenderer))]
public class PersonalWorkSpace : MonoBehaviour
{
    [Header("Reference")]
    public ViewManager VM;
    public VRTK_ControllerEvents rightCE;
    public VRTK_ControllerEvents leftCE;
    public Transform bottomRow;
    public Transform middleRow;
    public Transform topRow;
    public GameObject ObjectPrefab;

    [Header("Variables")]
    public float ObjectSize = 0.5f;
    public float ObjectDistance = 0.1f;
    public float UserHeightOffset = 1.8f;
    public float WorkSpaceHeight = 1f;
    public float minRadius = 0.7f;
    public int smoothDelta = 10;
    public float animationSpeed = 10;
    public float angleOffset = -90; // need to switch it back to private when foot is enabled
    public float rotationOffset = 0;


    [Header("Rows")]
    public int minObjectNumberBaseRow;
    public int maxObjectNumberTotal;
    public float rowHeightOffset = 0;
    public float RowHeightOffsetStep = 0.1f;
    public float maxRowHeightOffset = 0.5f;

    [Header("Control")]
    public string SlideLeft = "a";
    public string SlideRight = "d";
    public string SlideFront = "w";
    public string SlideBack = "s";

    private Transform User;
    private Transform Waist;
    // Start is called before the first frame update
    private void Awake()
    {
        User = VM.User;
        Waist = VM.Waist;

        angleOffset += User.localEulerAngles.y;
        rotationOffset = transform.localEulerAngles.y;

        ObjectDistance += ObjectSize;
        minObjectNumberBaseRow = (int)Mathf.Ceil(2 * minRadius * Mathf.PI / ObjectDistance);

    }

    private void Update()
    {
        rotationOffset = Waist.localEulerAngles.y;

        if (Input.GetKey(SlideRight) || rightCE.buttonOnePressed)  // slide right
        {
            angleOffset += 0.5f;
        }

        if (Input.GetKey(SlideLeft) || (rightCE.AnyButtonPressed() && !rightCE.buttonOnePressed)) // slide left
        {
            angleOffset -= 0.5f;
        }

        if (Input.GetKeyDown(SlideFront) || leftCE.buttonOnePressed) // slide front
        {
            DecreaseRadius();
            //ObjectSize += 0.05f;
            //ObjectDistance += 0.05f;
        }

        if (Input.GetKeyDown(SlideBack) || (leftCE.AnyButtonPressed() && !leftCE.buttonOnePressed)) // slide back
        {
            IncreaseRadius();
            //ObjectSize -= 0.05f;
            //ObjectDistance -= 0.05f;
        }

        if (Input.GetKeyDown("z")) 
        {
            GameObject go = Instantiate(ObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            go.name = "test object";
            go.transform.SetParent(bottomRow);
            go.transform.localScale = Vector3.one;
            // setup vis model
            Vis_PersonalWorkSpace newVis = new Vis_PersonalWorkSpace(go.name)
            {
                OnPrivateSpace = true
            };
            go.GetComponent<Vis_PersonalWorkSpace>().CopyEntity(newVis);
        }


        if (Waist.transform.position == Vector3.zero)
        {
            //transform.position = new Vector3(User.position.x, 0, User.position.z);
            WorkSpaceHeight = 1;
            //transform.rotation = User.rotation;
            //transform.localEulerAngles = new Vector3(0, User.localEulerAngles.y, 0);
        }
        else {
            //transform.position = new Vector3(Waist.position.x, 0, Waist.position.z);
            WorkSpaceHeight = Waist.transform.position.y;
            //transform.localEulerAngles = new Vector3(0, Waist.localEulerAngles.y, 0);
            //transform.localEulerAngles = Waist.localEulerAngles;
        }
    }


    private void DecreaseRadius()
    {
        if (topRow.childCount < topRow.GetComponent<Personal_Row>().numberLimit) {
            if (bottomRow.childCount > 0 && bottomRow.childCount > minObjectNumberBaseRow)
            {
                Transform t = bottomRow.GetChild(bottomRow.childCount - 1);

                if (middleRow.childCount == middleRow.GetComponent<Personal_Row>().numberLimit)
                {
                    if (topRow.GetComponent<Personal_Row>().numberLimit - topRow.childCount > 2)
                    {
                        middleRow.GetChild(middleRow.childCount - 1).SetParent(topRow);
                        t.SetParent(topRow);
                    }
                    else
                    {
                        Debug.Log("1");
                        if (rowHeightOffset < maxRowHeightOffset)
                            rowHeightOffset += RowHeightOffsetStep;
                    }
                }
                else
                {
                    if (middleRow.childCount < middleRow.GetComponent<Personal_Row>().numberLimit)
                    {
                        //Debug.Log((middleRow.childCount + 1) + " " + middleRow.GetComponent<Personal_Row>().numberLimit);
                        if ((middleRow.GetComponent<Personal_Row>().numberLimit - middleRow.childCount <= 1) && bottomRow.childCount > minObjectNumberBaseRow)
                        {
                            if (topRow.childCount < topRow.GetComponent<Personal_Row>().numberLimit - 1)
                            {
                                t.SetParent(topRow);
                            }
                            else
                            {
                                Debug.Log("2");
                                if (rowHeightOffset < maxRowHeightOffset)
                                    rowHeightOffset += RowHeightOffsetStep;
                            }
                        }
                        else
                        {
                            t.SetParent(middleRow);
                            if (middleRow.childCount == middleRow.GetComponent<Personal_Row>().numberLimit && topRow.childCount < topRow.GetComponent<Personal_Row>().numberLimit)
                            {
                                middleRow.GetChild(middleRow.childCount - 1).SetParent(topRow);
                            }
                        }
                    }
                    else if (topRow.childCount < topRow.GetComponent<Personal_Row>().numberLimit - 1)
                    {
                        t.SetParent(topRow);
                    }
                    else
                    {
                        Debug.Log("4");
                        if (rowHeightOffset < maxRowHeightOffset)
                            rowHeightOffset += RowHeightOffsetStep;
                    }
                }
            }
            else
            {
                Debug.Log("5");
                if (middleRow.childCount == middleRow.GetComponent<Personal_Row>().numberLimit && topRow.childCount < topRow.GetComponent<Personal_Row>().numberLimit)
                {
                    middleRow.GetChild(middleRow.childCount - 1).SetParent(topRow);
                }
                if (rowHeightOffset < maxRowHeightOffset)
                    rowHeightOffset += RowHeightOffsetStep;
            }
        }
        
        //else if(middleRow.childCount > 0) {
        //    if (topRow.childCount < topRow.GetComponent<Personal_Row>().numberLimit)
        //        middleRow.GetChild(middleRow.childCount - 1).SetParent(topRow);
        //}
    }
    private void IncreaseRadius() {
        if (rowHeightOffset > 0)
        {
            rowHeightOffset -= RowHeightOffsetStep;
        }
        else {
            if (topRow.childCount > 0)
            {
                if (middleRow.childCount < middleRow.GetComponent<Personal_Row>().numberLimit)
                    topRow.GetChild(topRow.childCount - 1).SetParent(middleRow);
                else
                    topRow.GetChild(topRow.childCount - 1).SetParent(bottomRow);

            }
            else if (middleRow.childCount > 0)
            {
                middleRow.GetChild(middleRow.childCount - 1).SetParent(bottomRow);
            }
        }
    }

    //private GameObject FindNearestObj(Transform user, LineRenderer lr)
    //{

    //    float maxDist = 10000;
    //    GameObject nearestGO = null;

    //    Vector3 userPosition2D = new Vector3(user.position.x, 0, user.position.z);


    //    foreach (GameObject go in visList)
    //    {
    //        Vector3 visPosition2D = new Vector3(go.transform.position.x, 0, go.transform.position.z);
    //        if (Vector3.Distance(userPosition2D, visPosition2D) < maxDist)
    //        {
    //            maxDist = Vector3.Distance(userPosition2D, visPosition2D);
    //            nearestGO = go;
    //        }
    //    }

    //    return nearestGO;
    //}




    //private int FindIntersectionIndex(Transform user, LineRenderer lr)
    //{
    //    Vector3[] linePositions = new Vector3[lr.positionCount];
    //    lr.GetPositions(linePositions);

    //    float maxDist = 10000;
    //    Vector3 intersectionPoint = Vector3.zero;
    //    Vector3 userPosition2D = new Vector3(user.position.x, 0, user.position.z);
    //    int intersectionIndex = 0;
    //    int i = 0;
    //    foreach (Vector3 v in linePositions)
    //    {

    //        if (Vector3.Distance(userPosition2D, v) < maxDist)
    //        {
    //            maxDist = Vector3.Distance(userPosition2D, v);
    //            intersectionPoint = v;
    //            intersectionIndex = i;
    //        }
    //        i++;
    //    }

    //    return intersectionIndex;
    //}



    //public Vector3 GetPositionFromCircle(Vector3 v) {
    //    float n = (1.57f - v.x) / 3.14f * 500;
    //    Vector3 pos = lineRenderer.GetPosition((int) n);
    //    currentIndex = (int) n;
    //    return new Vector3(pos.x, v.y, pos.z);
    //}
}
