using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonMesh : MonoBehaviour
{
    public GameObject parent;
    void Start()
    {
        transform.SetParent(parent.transform);
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
