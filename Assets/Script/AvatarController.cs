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
        GetComponent<Rigidbody>().isKinematic = true;
        if (Input.GetKey(KeyCode.UpArrow)) // front
        {
            Avatar.localPosition += Avatar.forward * translationSpeed;
        }

        if (Input.GetKey(KeyCode.DownArrow)) // back
        {
            Avatar.localPosition -= Avatar.forward * translationSpeed;
        }

        if (Input.GetKey(KeyCode.RightArrow)) // right
        {
            Avatar.localPosition += Avatar.right * translationSpeed;
            //Avatar.localEulerAngles += Vector3.up * translationSpeed * 50;
        }

        if (Input.GetKey(KeyCode.LeftArrow)) // left
        {
            Avatar.localPosition -= Avatar.right * translationSpeed;
            //Avatar.localEulerAngles -= Vector3.up * translationSpeed * 50;
        }
    }
}
