using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewManager : MonoBehaviour
{
    [Header("Prefab")]
    public Transform User;
    public Transform Waist;
    public GameObject ObjectPrefab;
    public Transform PublicWorkSpace;
    public Transform PrivateWorkSpace;
    public PersonalWorkSpace PersonalWorkSpaceManager;

    [Header("Variables")]
    public float ObjectNumber = 100;


    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < ObjectNumber; i++)
        {
            GameObject go = Instantiate(ObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            go.name = "object " + (i + 1);
            go.transform.SetParent(PublicWorkSpace);
            go.transform.localScale = Vector3.one;
            // setup vis model
            Vis_PersonalWorkSpace newVis = new Vis_PersonalWorkSpace(go.name)
            {
                OnPublicSpace = true
            };
            go.GetComponent<Vis_PersonalWorkSpace>().CopyEntity(newVis);

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    } 
}
