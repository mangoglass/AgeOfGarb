using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPTP : MonoBehaviour
{

    public GameObject t;
    // Start is called before the first frame update
    void Start()
    {
        MapCreator mc = GetComponentInChildren<MapCreator>();
        mc.CreateMap(t);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
