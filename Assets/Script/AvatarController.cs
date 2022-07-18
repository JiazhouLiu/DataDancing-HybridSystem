using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public Transform Avatar;
    public ViewManager VM;
    public float translationSpeed = 1;
    public float rotationSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w")) // front
        {
            Avatar.localPosition += Avatar.forward * translationSpeed;
        }

        if (Input.GetKey("s")) // back
        {
            Avatar.localPosition -= Avatar.forward * translationSpeed;
        }

        if (Input.GetKey("d")) // right
        {
            Avatar.localPosition += Avatar.right * translationSpeed;
        }

        if (Input.GetKey("a")) // left
        {
            Avatar.localPosition -= Avatar.right * translationSpeed;
        }

        if (Input.GetKey("e")) // turn left
        {
            Avatar.localEulerAngles = new Vector3(Avatar.localEulerAngles.x, Avatar.localEulerAngles.y + rotationSpeed, Avatar.localEulerAngles.z);
            VM.CheckRotation();
        }

        if (Input.GetKey("q")) // turn right
        {
            Avatar.localEulerAngles = new Vector3(Avatar.localEulerAngles.x, Avatar.localEulerAngles.y - rotationSpeed, Avatar.localEulerAngles.z);
            VM.CheckRotation();
        }
    }
}
