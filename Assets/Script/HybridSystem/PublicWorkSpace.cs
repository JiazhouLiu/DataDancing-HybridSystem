using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

enum Layout
{
    Flat,
    Semicircle,
    Fullcircle,
    Square
}


public class PublicWorkSpace : MonoBehaviour
{
    [Header("Reference")]
    public ViewManager VM;
    public VRTK_ControllerEvents leftCE;

    [Header("Variables")]
    public float ObjectSize = 0.5f;
    public float ObjectDistance = 0.1f;
    public float movingSpeed = 10;


    [Header("Control")]
    public string SideLeft = "j";
    public string SideRight = "l";
    public string RotationLeft = "u";
    public string RotationRight = "o";

    private Transform User;
    private Transform Waist;

    private float currentObjectNumber;
    private float previousObjectNumber = 0;
    private float numRow;
    private float WorkSpaceHeight;

    private List<Transform> visList;
    private List<Vector3> visPositionList;
    private List<Vector3> visRotationList;
    private Layout currentLayout = Layout.Flat;


    // Start is called before the first frame update
    void Awake()
    {
        User = VM.User;
        Waist = VM.Waist;

        visList = new List<Transform>();
        visPositionList = new List<Vector3>();
        visRotationList = new List<Vector3>();

        InitiateViews();

        if (Waist.transform.position == Vector3.zero)
            WorkSpaceHeight = 1;
        else
            WorkSpaceHeight = Waist.transform.position.y;

        transform.localPosition += Vector3.up * WorkSpaceHeight;
    }

    // Update is called once per frame
    void Update()
    {
        currentObjectNumber = transform.childCount;

        if (currentObjectNumber != previousObjectNumber) {
            InitiateViews();
            numRow = (int)Mathf.Sqrt(currentObjectNumber);

            float numCol = Mathf.Ceil(currentObjectNumber / numRow);
            visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
            visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            foreach (Transform t in visList) {
                t.localScale = Vector3.one * ObjectSize;
            }
        }
            

        //Debug.Log(User.localEulerAngles.y);
        if (Input.GetKeyDown(SideRight) || leftCE.buttonOnePressed)  // slide right
        {
            if (numRow > 1)
            {
                numRow--;
                float numCol = Mathf.Ceil(currentObjectNumber / numRow);
                Debug.Log(numCol);
                visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
                visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            }
        }

        if (Input.GetKeyDown(SideLeft) || (leftCE.AnyButtonPressed() && !leftCE.buttonOnePressed)) // slide left
        {
            if (numRow < currentObjectNumber)
            {
                numRow++;
                float numCol = Mathf.Ceil(currentObjectNumber / numRow);
                visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
                visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            }
        }

        if (Input.GetKeyDown("t")) // testing
        {
            float numCol = Mathf.Ceil(currentObjectNumber / numRow);
            if (numCol > 4 && currentLayout == Layout.Fullcircle)
            {
                currentLayout = Layout.Square;
                visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
                visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            }
            else if (currentLayout == Layout.Square)
            {
                currentLayout = Layout.Fullcircle;
                visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
                visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            }
        }

        if (Input.GetKeyDown(RotationLeft)) // toe rotation left
        {
            if (currentLayout == Layout.Fullcircle)
            {
                currentLayout = Layout.Semicircle;
                float numCol = Mathf.Ceil(currentObjectNumber / numRow);
                visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
                visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            }
            else if (currentLayout == Layout.Semicircle)
            {
                currentLayout = Layout.Flat;
                float numCol = Mathf.Ceil(currentObjectNumber / numRow);
                visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
                visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            }
        }

        if (Input.GetKeyDown(RotationRight)) // toe rotation right
        {
            if (currentLayout == Layout.Flat)
            {
                currentLayout = Layout.Semicircle;
                float numCol = Mathf.Ceil(currentObjectNumber / numRow);
                visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
                visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            }
            else if (currentLayout == Layout.Semicircle)
            {
                currentLayout = Layout.Fullcircle;
                float numCol = Mathf.Ceil(currentObjectNumber / numRow);
                visPositionList = UpdateObjectPositions(visList, numRow, numCol, currentLayout);
                visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
            }
        }


        previousObjectNumber = currentObjectNumber;
        MoveObjects();
    }

    public void CheckRotation()
    {
        if (currentLayout == Layout.Square)
        {
            float numCol = Mathf.Ceil(currentObjectNumber / numRow);
            visRotationList = UpdateObjectRotations(visList, numRow, numCol, currentLayout);
        }
    }

    private List<Vector3> UpdateObjectRotations(List<Transform> list, float numRow, float numCol, Layout layout)
    {
        List<Vector3> visRotations = new List<Vector3>();

        if (layout == Layout.Flat)
        {

            int ListIndex = 0;
            for (int j = (int)numRow - 1; j >= 0; j--)
            {
                for (int i = 0; i < (int)numCol; i++)
                {
                    if (ListIndex < list.Count)
                    {
                        visRotations.Add(Vector3.zero);
                        ListIndex++;
                    }
                }
            }
        }
        else if (layout == Layout.Fullcircle)
        {
            int ListIndex = 0;
            for (int j = (int)numRow - 1; j >= 0; j--)
            {
                for (int i = 0; i < (int)numCol; i++)
                {
                    if (ListIndex < list.Count)
                    {
                        float yValue = (j - 0.5f) * ObjectSize + (j - 1) * ObjectDistance;

                        Vector3 centerPoint = transform.parent.TransformPoint(transform.parent.position);
                        GameObject tmp = new GameObject();
                        tmp.transform.SetParent(transform.parent);
                        tmp.transform.localPosition = visPositionList[ListIndex];

                        tmp.transform.LookAt(new Vector3(centerPoint.x, yValue, centerPoint.z));
                        tmp.transform.localEulerAngles += Vector3.up * 180;

                        visRotations.Add(tmp.transform.localEulerAngles);
                        Destroy(tmp);

                        ListIndex++;
                    }
                }
            }

        }
        else if (layout == Layout.Semicircle)
        {
            int ListIndex = 0;
            for (int j = (int)numRow - 1; j >= 0; j--)
            {
                for (int i = 0; i < (int)numCol; i++)
                {
                    if (ListIndex < list.Count)
                    {
                        float yValue = (j - 0.5f) * ObjectSize + (j - 1) * ObjectDistance;

                        Vector3 centerPoint = transform.parent.TransformPoint(transform.parent.position);
                        GameObject tmp = new GameObject();
                        tmp.transform.SetParent(transform.parent);
                        tmp.transform.localPosition = visPositionList[ListIndex];

                        tmp.transform.LookAt(new Vector3(centerPoint.x, yValue, centerPoint.z));
                        tmp.transform.localEulerAngles += Vector3.up * 180;

                        visRotations.Add(tmp.transform.localEulerAngles);
                        Destroy(tmp);

                        ListIndex++;
                    }
                }
            }
        }
        else if (layout == Layout.Square)
        {
            int ListIndex = 0;


            int numExtraColPerSide = 0;
            int howManySidesHaveExtra = 0;

            if (numCol == 4)
            {
                numExtraColPerSide = 0;
                howManySidesHaveExtra = 0;
            }
            else
            {
                numExtraColPerSide = ((int)numCol - 1) / 4;
                howManySidesHaveExtra = ((int)numCol - 1) % 4 + 1;
            }

            for (int j = (int)numRow - 1; j >= 0; j--)
            {
                for (int i = 0; i < (int)numCol; i++)
                {
                    if (ListIndex < list.Count)
                    {
                        Vector3 finalRotation = Vector3.zero;
                        if (i < 2 + numExtraColPerSide)
                        { // front
                            finalRotation = Vector3.zero;
                        }
                        else if (howManySidesHaveExtra == 1 && i < 2 + 2 * numExtraColPerSide)
                        { // right
                            finalRotation = new Vector3(0, 90, 0);
                        }
                        else if (howManySidesHaveExtra != 1 && i < 3 + 2 * numExtraColPerSide)
                        { // right
                            finalRotation = new Vector3(0, 90, 0);
                        }
                        else if (howManySidesHaveExtra == 1 && i < 2 + 3 * numExtraColPerSide)
                        { // back
                            finalRotation = new Vector3(0, 180, 0);
                        }
                        else if (howManySidesHaveExtra == 2 && i < 3 + 3 * numExtraColPerSide)
                        { // back
                            finalRotation = new Vector3(0, 180, 0);
                        }
                        else if (howManySidesHaveExtra != 1 && howManySidesHaveExtra != 2 && i < 4 + 3 * numExtraColPerSide)
                        {// back
                            finalRotation = new Vector3(0, 180, 0);
                        }
                        else
                        { // left
                            finalRotation = new Vector3(0, 270, 0);
                        }

                        if (howManySidesHaveExtra == 1)
                        {
                            if (User.localEulerAngles.y >= 315 || User.localEulerAngles.y <= 45)
                            { // look forward
                                if (i == 0 || i == 1 + numExtraColPerSide)
                                    finalRotation = Vector3.zero;
                            }
                            else if (User.localEulerAngles.y > 45 && User.localEulerAngles.y <= 135)
                            { // look right
                                if (i == 1 + numExtraColPerSide || i == 1 + 2 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 90, 0);
                            }
                            else if (User.localEulerAngles.y > 135 && User.localEulerAngles.y <= 225)
                            { //  look back
                                if (i == 1 + 2 * numExtraColPerSide || i == 1 + 3 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 180, 0);
                            }
                            else
                            {
                                if (i == 0 || i == 1 + 3 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 270, 0);
                            }
                        }
                        else if (howManySidesHaveExtra == 2)
                        {
                            if (User.localEulerAngles.y >= 315 || User.localEulerAngles.y <= 45)
                            { // look forward
                                if (i == 0 || i == 1 + numExtraColPerSide)
                                    finalRotation = Vector3.zero;
                            }
                            else if (User.localEulerAngles.y > 45 && User.localEulerAngles.y <= 135)
                            { // look right
                                if (i == 1 + numExtraColPerSide || i == 2 + 2 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 90, 0);
                            }
                            else if (User.localEulerAngles.y > 135 && User.localEulerAngles.y <= 225)
                            { //  look back
                                if (i == 2 + 2 * numExtraColPerSide || i == 2 + 3 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 180, 0);
                            }
                            else
                            {
                                if (i == 0 || i == 2 + 3 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 270, 0);
                            }
                        }
                        else
                        {
                            if (User.localEulerAngles.y >= 315 || User.localEulerAngles.y <= 45)
                            { // look forward
                                if (i == 0 || i == 1 + numExtraColPerSide)
                                    finalRotation = Vector3.zero;
                            }
                            else if (User.localEulerAngles.y > 45 && User.localEulerAngles.y <= 135)
                            { // look right
                                if (i == 1 + numExtraColPerSide || i == 2 + 2 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 90, 0);
                            }
                            else if (User.localEulerAngles.y > 135 && User.localEulerAngles.y <= 225)
                            { //  look back
                                if (i == 2 + 2 * numExtraColPerSide || i == 3 + 3 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 180, 0);
                            }
                            else
                            {
                                if (i == 0 || i == 3 + 3 * numExtraColPerSide)
                                    finalRotation = new Vector3(0, 270, 0);
                            }
                        }

                        visRotations.Add(finalRotation);
                    }
                }
            }
        }

        return visRotations;
    }

    private List<Vector3> UpdateObjectPositions(List<Transform> list, float numRow, float numCol, Layout layout)
    {
        List<Vector3> visPositions = new List<Vector3>();

        if (layout == Layout.Flat)
        {
            int ListIndex = 0;
            for (int j = (int)numRow - 1; j >= 0; j--)
            {
                for (int i = 0; i < (int)numCol; i++)
                {
                    if (ListIndex < list.Count)
                    {
                        Vector3 newPosition = new Vector3((i - numCol / 2) * (ObjectDistance + ObjectSize), (j - 0.5f) * ObjectSize + (j - 1) * ObjectDistance, 0);
                        visPositions.Add(newPosition);

                        ListIndex++;
                    }

                }
            }
        }
        else if (layout == Layout.Fullcircle)
        {
            int ListIndex = 0;
            for (int j = (int)numRow - 1; j >= 0; j--)
            {
                for (int i = 0; i < (int)numCol; i++)
                {
                    if (ListIndex < list.Count)
                    {
                        float xValue = -Mathf.Cos(i * 2 * Mathf.PI / (numCol - 1)) * (numCol * (ObjectSize + ObjectDistance) / (2 * Mathf.PI));
                        float yValue = (j - 0.5f) * ObjectSize + (j - 1) * ObjectDistance;
                        float zValue = Mathf.Sin(i * 2 * Mathf.PI / (numCol - 1)) * (numCol * (ObjectSize + ObjectDistance) / (2 * Mathf.PI));
                        visPositions.Add(new Vector3(xValue, yValue, zValue));

                        ListIndex++;
                    }
                }
            }

        }
        else if (layout == Layout.Semicircle)
        {
            int ListIndex = 0;
            for (int j = (int)numRow - 1; j >= 0; j--)
            {
                for (int i = 0; i < (int)numCol; i++)
                {
                    if (ListIndex < list.Count)
                    {
                        float xValue = -Mathf.Cos(i * Mathf.PI / (numCol - 1)) * ((numCol - 1) * (ObjectSize + ObjectDistance) / (Mathf.PI));
                        float yValue = (j - 0.5f) * ObjectSize + (j - 1) * ObjectDistance;
                        float zValue = Mathf.Sin(i * Mathf.PI / (numCol - 1)) * ((numCol - 1) * (ObjectSize + ObjectDistance) / (Mathf.PI));

                        visPositions.Add(new Vector3(xValue, yValue, zValue));

                        ListIndex++;
                    }
                }
            }
        }
        else if (layout == Layout.Square)
        {
            int ListIndex = 0;

            float xValue = 0;
            float yValue = 0;
            float zValue = 0;

            int numExtraColPerSide = 0;
            int howManySidesHaveExtra = 0;

            if (numCol == 4)
            {
                numExtraColPerSide = 0;
                howManySidesHaveExtra = 0;
            }
            else
            {
                numExtraColPerSide = ((int)numCol - 1) / 4;
                howManySidesHaveExtra = ((int)numCol - 1) % 4 + 1;
            }
            //Debug.Log("numExtraColPerSide " + numExtraColPerSide);
            //Debug.Log("howManySidesHaveExtra " + howManySidesHaveExtra);

            for (int j = (int)numRow - 1; j >= 0; j--)
            {
                for (int i = 0; i < (int)numCol; i++)
                {
                    if (ListIndex < list.Count)
                    {
                        if (i < 2 + numExtraColPerSide)
                        { // front
                            //Debug.Log(i + " front");
                            xValue = (i - ((float)numExtraColPerSide + 1) / 2) * (ObjectDistance + ObjectSize);
                            zValue = ((float)numExtraColPerSide + 1) / 2 * (ObjectDistance + ObjectSize);
                        }
                        else if (howManySidesHaveExtra == 1 && i < 2 + 2 * numExtraColPerSide)
                        { // right
                            //Debug.Log(i + " right");
                            xValue = ((float)numExtraColPerSide + 1) / 2 * (ObjectDistance + ObjectSize);
                            if (i == 1 + 2 * numExtraColPerSide)
                                zValue = ((1.5f * numExtraColPerSide + 0.5f) - i) * (ObjectDistance + ObjectSize);
                            else
                                zValue = ((1.5f * numExtraColPerSide + 1.5f) - i) * (ObjectDistance + ObjectSize);
                        }
                        else if (howManySidesHaveExtra != 1 && i < 3 + 2 * numExtraColPerSide)
                        { // right
                            //Debug.Log(i + " right");
                            xValue = ((float)numExtraColPerSide + 1) / 2 * (ObjectDistance + ObjectSize);
                            zValue = ((1.5f * numExtraColPerSide + 1.5f) - i) * (ObjectDistance + ObjectSize);
                        }
                        else if (howManySidesHaveExtra == 1 && i < 2 + 3 * numExtraColPerSide)
                        { // back
                            //Debug.Log(i + " back");
                            if (i == 1 + 3 * numExtraColPerSide)
                                xValue = ((2.5f * numExtraColPerSide + 0.5f) - i) * (ObjectDistance + ObjectSize);
                            else
                                xValue = ((2.5f * numExtraColPerSide + 1.5f) - i) * (ObjectDistance + ObjectSize);

                            zValue = -((float)numExtraColPerSide + 1) / 2 * (ObjectDistance + ObjectSize);
                        }
                        else if (howManySidesHaveExtra == 2 && i < 3 + 3 * numExtraColPerSide)
                        { // back
                            //Debug.Log(i + " back");
                            if (i == 2 + 3 * numExtraColPerSide)
                                xValue = ((2.5f * numExtraColPerSide + 1.5f) - i) * (ObjectDistance + ObjectSize);
                            else
                                xValue = ((2.5f * numExtraColPerSide + 2.5f) - i) * (ObjectDistance + ObjectSize);

                            zValue = -((float)numExtraColPerSide + 1) / 2 * (ObjectDistance + ObjectSize);
                        }
                        else if (howManySidesHaveExtra != 1 && howManySidesHaveExtra != 2 && i < 4 + 3 * numExtraColPerSide)
                        {// back
                            //Debug.Log(i + " back");
                            xValue = ((2.5f * numExtraColPerSide + 2.5f) - i) * (ObjectDistance + ObjectSize);
                            zValue = -((float)numExtraColPerSide + 1) / 2 * (ObjectDistance + ObjectSize);
                        }
                        else
                        { // left
                            //Debug.Log(i + " left");
                            xValue = -((float)numExtraColPerSide + 1) / 2 * (ObjectDistance + ObjectSize);
                            if (howManySidesHaveExtra == 4)
                                zValue = (i - (3.5f * numExtraColPerSide + howManySidesHaveExtra - 0.5f)) * (ObjectDistance + ObjectSize);
                            else
                                zValue = (i - (3.5f * numExtraColPerSide + howManySidesHaveExtra + 0.5f)) * (ObjectDistance + ObjectSize);
                        }
                        yValue = (j - 0.5f) * ObjectSize + (j - 1) * ObjectDistance;

                        visPositions.Add(new Vector3(xValue, yValue, zValue));

                        ListIndex++;
                    }
                }

            }
        }

        return visPositions;
    }

    private void MoveObjects()
    {
        for (int i = 0; i < visList.Count; i++)
        {
            //Debug.Log(visPositionList.Count);
            visList[i].localPosition = Vector3.Lerp(visList[i].localPosition, visPositionList[i], Time.deltaTime * movingSpeed);
            
            visList[i].rotation = Quaternion.Lerp(visList[i].rotation, Quaternion.Euler(visRotationList[i]), Time.deltaTime * movingSpeed);
        }
    }

    private void InitiateViews()
    {

        visList.Clear();

        foreach (Transform child in transform)
        {
            visList.Add(child);
        }
    }
}
