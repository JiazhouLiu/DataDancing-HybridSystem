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

    private bool visHighlighted = false;
    private List<GameObject> list;

    // Start is called before the first frame update
    void Awake()
    {
        list = new List<GameObject>();

        for (int i = 0; i < ObjectNumber; i++)
        {
            GameObject go = Instantiate(ObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            Material m = Resources.Load("VIS/non-highlighted/mat/h" + (i+1), typeof(Material)) as Material;
            go.transform.GetChild(0).GetComponent<MeshRenderer>().material = m;

            go.name = "object " + (i + 1);
            go.transform.SetParent(PublicWorkSpace);
            go.transform.localScale = Vector3.one;
            // setup vis model
            Vis_PersonalWorkSpace newVis = new Vis_PersonalWorkSpace(go.name)
            {
                OnPublicSpace = true
            };
            go.GetComponent<Vis_PersonalWorkSpace>().CopyEntity(newVis);

            list.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeVis() {
        if (!visHighlighted)
        {
            visHighlighted = true;

            int i = 1;
            foreach (GameObject go in list)
            {
                Material m = Resources.Load("VIS/highlighted/mat/h" + i, typeof(Material)) as Material;
                go.transform.GetChild(0).GetComponent<MeshRenderer>().material = m;

                i++;
            }
        }
        else {
            visHighlighted = false;

            int i = 1;
            foreach (GameObject go in list)
            {
                Material m = Resources.Load("VIS/non-highlighted/mat/h" + i, typeof(Material)) as Material;
                go.transform.GetChild(0).GetComponent<MeshRenderer>().material = m;

                i++;
            }
        }
    }
}
