using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Vis_PersonalWorkSpace : MonoBehaviour
{
    public Transform VisBorder;
    public string showName;
    public Vector3 showPosition;
    public Vector3 showScale;
    public bool showOnPublicSpace;
    public bool showOnPrivateSpace;
    public bool showHighlighted;
    public bool showSelected;
    public bool showMoving;

    public string VisName { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }

    public bool OnPublicSpace { get; set; }
    public bool OnPrivateSpace { get; set; }
    public bool Highlighted { get; set; }
    public bool Selected { get; set; }
    public bool Moving { get; set; }

    public Vis_PersonalWorkSpace() { }

    public Vis_PersonalWorkSpace(string name, Vector3 position, Vector3 scale)
    {
        VisName = name;
        Position = position;
        Scale = scale;
    }

    public Vis_PersonalWorkSpace(string name)
    {
        VisName = name;
    }

    public void CopyEntity(Vis_PersonalWorkSpace v)
    {
        VisName = v.VisName;
        Position = v.Position;
        Scale = v.Scale;

        OnPublicSpace = v.OnPublicSpace;
        OnPrivateSpace = v.OnPrivateSpace;

        Highlighted = v.Highlighted;
        Selected = v.Selected;
        Moving = v.Moving;

        showName = VisName;
        showPosition = Position;
        showScale = Scale;

        showOnPublicSpace = OnPublicSpace;
        showOnPrivateSpace = OnPrivateSpace;

        showHighlighted = Highlighted;
        showSelected = Selected;
        showMoving = Moving;
    }

    public void Update()
    {
        showName = VisName;
        showPosition = Position;
        showScale = Scale;

        showOnPublicSpace = OnPublicSpace;
        showOnPrivateSpace = OnPrivateSpace;

        showHighlighted = Highlighted;
        showSelected = Selected;
        showMoving = Moving;
    }
}
