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

        Vector3[] trash_positions = new Vector3[] {
            new Vector3(-8f, 0f, 10f),
            new Vector3(12f, 0f, 13f),
            new Vector3(-11f, 0f, -10f),
            new Vector3(18f, 0f, -16f)
        };
        mc.CreateMapTrashCans(trash_positions, t);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
