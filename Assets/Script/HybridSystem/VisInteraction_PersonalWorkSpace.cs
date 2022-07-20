using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using VRTK;
using UnityEngine.UI;
using DG.Tweening;

enum InteractionType { 
    Select,
    Grab
}

public class VisInteraction_PersonalWorkSpace : MonoBehaviour
{
    [SerializeField]
    private ViewManager VM;
    [SerializeField]
    private VRTK_InteractableObject interactableObject;
    [SerializeField]
    private Vis_PersonalWorkSpace visualisation;
    [SerializeField]
    private BoxCollider currentBoxCollider;
    [SerializeField]
    private Rigidbody currentRigidbody;
    [SerializeField]
    private InteractionType interactionType;

    public bool isGrabbing = false;
    private bool isTouchingDisplaySurface = false;
    private Transform touchingSurface;

    private Transform previousParent;
    private int previousSiblingIndex;

     
    private void Awake()
    {
        VM = GameObject.Find("ViewManager").GetComponent<ViewManager>();
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
        previousParent = transform.parent;
        previousSiblingIndex = transform.GetSiblingIndex();
        transform.SetParent(null);

        visualisation.OnPrivateSpace = false;
        visualisation.OnPublicSpace = false;

        visualisation.showHighlighted = false;
        visualisation.Moving = true;
    }

    private void VisUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        isGrabbing = false;

        visualisation.Moving = false;

        if (isTouchingDisplaySurface)
            AttachToDisplaySurface();
        else
        {
            transform.SetParent(previousParent);
            transform.SetSiblingIndex(previousSiblingIndex);
        }
    }

    private void VisUsed(object sender, InteractableObjectEventArgs e)
    {
        if (GetComponent<Vis_PersonalWorkSpace>().Selected) // from personal to public
        {
            visualisation.Selected = false;
            visualisation.OnPrivateSpace = false;
            visualisation.OnPublicSpace = true;

            AttachToDisplaySurface(VM.PublicWorkSpace);
        }
        else { // from public to personal
            visualisation.Selected = true;
            visualisation.OnPrivateSpace = true;
            visualisation.OnPublicSpace = false;

            AttachToDisplaySurface(VM.PrivateWorkSpace);
        }

    }

    private void OnDestroy()
    {
        //Unsubscribe to events
        interactableObject.InteractableObjectGrabbed -= VisGrabbed;
        interactableObject.InteractableObjectUngrabbed -= VisUngrabbed;
        interactableObject.InteractableObjectUsed -= VisUsed;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("DisplaySurface"))
        //{
        //    isTouchingDisplaySurface = true;
        //    touchingSurface = other.transform;
        //    currentRigidbody.isKinematic = true;
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DisplaySurface"))
        {
            isTouchingDisplaySurface = true;
            touchingSurface = other.transform;
            currentRigidbody.isKinematic = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DisplaySurface"))
            isTouchingDisplaySurface = false;
    }

    private void AttachToDisplaySurface() {
        transform.SetParent(touchingSurface);
        visualisation.Moving = false;

        if (touchingSurface.name == "Public")
        {
            visualisation.Selected = false;
            visualisation.OnPrivateSpace = false;
            visualisation.OnPublicSpace = true;
        }
        else if (touchingSurface.name == "Personal") {
            visualisation.Selected = true;
            visualisation.OnPrivateSpace = true;
            visualisation.OnPublicSpace = false;
        }
    }

    private void AttachToDisplaySurface(Transform targetT)
    {
        transform.SetParent(targetT);
        visualisation.Moving = false;
    }

    #endregion

    #region vis related
    private void ReturnToLastState()
    {
        transform.SetParent(previousParent);
        transform.SetSiblingIndex(previousSiblingIndex);
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
