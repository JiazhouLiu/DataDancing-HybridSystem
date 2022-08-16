using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vis_WrapAround : MonoBehaviour
{
    public Transform VisBorder;
    public string showName;
    public Vector3 showWallPosition;
    public Vector3 showTableCirclePosition;
    public Vector3 showBodyCirclePosition;
    public bool showOnWall;
    public bool showOnTable;
    public bool showOnBody;
    public bool showSelected;
    public bool showMoving;

    public string VisName { get; set; }
    public Vector3 WallPosition { get; set; }
    public Vector3 TableCirclePosition { get; set; }
    public Vector3 BodyCirclePosition { get; set; }

    public bool OnTable { get; set; }
    public bool OnWall { get; set; }
    public bool OnBody { get; set; }
    public bool Highlighted { get; set; }
    public bool Selected { get; set; }
    public bool Moving { get; set; }

    public Vis_WrapAround() { }

    public Vis_WrapAround(string name, Vector3 wallPosition, Vector3 tableCirclePosition, Vector3 bodyCirclePosition)
    {
        VisName = name;
        WallPosition = wallPosition;
        TableCirclePosition = tableCirclePosition;
        BodyCirclePosition = bodyCirclePosition;
    }

    public Vis_WrapAround(string name, Vector3 wallPosition)
    {
        VisName = name;
        WallPosition = wallPosition;
    }

    public Vis_WrapAround(string name)
    {
        VisName = name;
    }

    public void CopyEntity(Vis_WrapAround v)
    {
        VisName = v.VisName;
        WallPosition = v.WallPosition;
        BodyCirclePosition = v.BodyCirclePosition;
        TableCirclePosition = v.TableCirclePosition;

        OnBody = v.OnBody;
        OnTable = v.OnTable;
        OnWall = v.OnWall;

        Highlighted = v.Highlighted;
        Selected = v.Selected;
        Moving = v.Moving;

        showName = VisName;
        showWallPosition = WallPosition;
        showTableCirclePosition = TableCirclePosition;
        showBodyCirclePosition = BodyCirclePosition;

        showOnTable = OnTable;
        showOnWall = OnWall;
        showOnBody = OnBody;

        showSelected = Selected;
        showMoving = Moving;
    }

    public void Update()
    {
        showName = VisName;
        showWallPosition = WallPosition;
        showBodyCirclePosition = BodyCirclePosition;
        showTableCirclePosition = TableCirclePosition;

        showOnTable = OnTable;
        showOnWall = OnWall;
        showOnBody = OnBody;

        showSelected = Selected;
        showMoving = Moving;

        if (Selected)
        {
            foreach (Transform t in VisBorder)
                t.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else {
            foreach (Transform t in VisBorder)
                t.GetComponent<MeshRenderer>().material.color = Color.white;
        }

        if (OnTable || OnBody)
        {
            transform.localScale = Vector3.one * 0.2f;
        }
        else if (OnWall) {
            transform.localScale = Vector3.one * 0.3f;
        }

    }



    public Vector3 ConvertPositions(Vector3 pos, string surface) {
        Vector3 otherPosition = Vector3.zero;
        if (surface == "Wall")
        {
            otherPosition = new Vector3(pos.x, pos.y / 1.5f, 0);
        }
        else if (surface == "Table") {
            otherPosition = new Vector3(pos.x, pos.z * 1.5f, 0);
        }

        return otherPosition;
    }

    public Vector3 GetCurrentPosition() {
        if (OnWall)
            return WallPosition;
        else if (OnTable)
            return TableCirclePosition;
        else if (OnBody)
            return BodyCirclePosition;
        else
            return Vector3.zero;
    }

    public void SetCurrentPosition(Vector3 pos) {
        if (OnWall)
            WallPosition = pos;
        else if (OnTable)
            TableCirclePosition = pos;
        else if (OnBody)
            BodyCirclePosition = pos;
    }
}
