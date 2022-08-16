using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;
using DG.Tweening;


public class VisInteraction_WrapAround : MonoBehaviour
{
    [SerializeField]
    private VRTK_InteractableObject interactableObject;
    [SerializeField]
    private Vis_WrapAround visualisation;
    [SerializeField]
    private Rigidbody currentRigidbody;


    public bool isGrabbing = false;
    private bool isTouchingDisplaySurface = false;

    private void Awake()
    {
        // Subscribe to events
        interactableObject.InteractableObjectGrabbed -= VisGrabbed;
        interactableObject.InteractableObjectGrabbed += VisGrabbed;
        interactableObject.InteractableObjectUngrabbed -= VisUngrabbed;
        interactableObject.InteractableObjectUngrabbed += VisUngrabbed;

        interactableObject.InteractableObjectUsed -= VisUsed;
        interactableObject.InteractableObjectUsed += VisUsed;
    }

    private void Update()
    {

    }

    #region Interaction Event: Trigger, grabbing, detection
    private void VisGrabbed(object sender, InteractableObjectEventArgs e)
    {
        isGrabbing = true;
        visualisation.Moving = true;
    }

    private void VisUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        isGrabbing = false;
        visualisation.Moving = false;

        if (isTouchingDisplaySurface)
            AttachToDisplaySurface();
        else {
            transform.position = visualisation.GetCurrentPosition();
            transform.localEulerAngles = Vector3.zero;
        } 
    }

    private void VisUsed(object sender, InteractableObjectEventArgs e)
    {
        if (visualisation.Selected)
            visualisation.Selected = false;
        else
            visualisation.Selected = true;
    }

    private void OnDestroy()
    {
        //Unsubscribe to events
        interactableObject.InteractableObjectGrabbed -= VisGrabbed;
        interactableObject.InteractableObjectUngrabbed -= VisUngrabbed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DisplaySurface"))
        {
            isTouchingDisplaySurface = true;
            currentRigidbody.isKinematic = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DisplaySurface"))
            isTouchingDisplaySurface = false;
    }

    private void AttachToDisplaySurface()
    {
        visualisation.Moving = false;
        visualisation.Selected = false;

        visualisation.SetCurrentPosition(transform.position);
        transform.localEulerAngles = Vector3.zero;
    }

    #endregion

    #region Utilities
    public void AnimateTowards(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        ColliderActiveState = false;

        transform.DOLocalMove(targetPos, duration).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            ColliderActiveState = true;
        });

        transform.DOLocalRotate(targetRot.eulerAngles, duration).SetEase(Ease.OutQuint);
    }

    public bool ColliderActiveState
    {
        get { return GetComponent<Collider>().enabled; }
        set
        {
            if (value == ColliderActiveState)
                return;
        }
    }
    #endregion
}
