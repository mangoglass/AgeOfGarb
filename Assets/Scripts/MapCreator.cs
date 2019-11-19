using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    public GameObject[] buildings;
    public GameObject[] trees;
    float minDist = 7f;
    float scale = 1f;

    public GameObject temporary;

    // Start is called before the first frame update
    void Start()
    {

        Vector3[] points = new Vector3[]{
            new Vector3(-40,0,-40),
            new Vector3(-40,0,40),
            new Vector3(40,0,40),
            new Vector3(40,0,-40),

        };
        CreateMap(points, temporary);
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateMap(Vector3[] points, GameObject parent)
    {

        CreateBuildingOutline(points, parent);
        CreateTreesInSquare(points[0] + new Vector3(minDist, 0, minDist), points[2] - new Vector3(minDist, 0, minDist), 40, parent);

    }

    void CreateTreesInSquare(Vector3 a, Vector3 b, int n, GameObject parent)
    {
        Vector3 diff = (b - a);

        float xRange = diff.x;
        float zRange = diff.z;

        for (int i = 0; i < n; i++) {
            float xf = Random.Range(0f, 1f);
            float zf = Random.Range(0f, 1f);

            Vector3 pos = a + new Vector3(xf * xRange, 0, zf * zRange);
            CreateTree(pos, Quaternion.identity, scale,parent);

        }


    }

    void CreateBuildingOutline(Vector3[] points, GameObject parent)
    {

        //Vector3 minDistVec = new Vector3(minDist, 0, minDist);
        for (int i = 0; i < points.Length; i++)
        {

            Vector3 a = points[i];
            Vector3 b = points[(i + 1) % points.Length];

            Vector3 diff = (b - a);

            //int n = Mathf.Min(Mathf.FloorToInt(diff.x / minDist), Mathf.FloorToInt(diff.z / minDist));
            int n = Mathf.FloorToInt(diff.magnitude / minDist);

            float xStep = diff.x / n;
            float zStep = diff.z / n;


            for (int j = 0; j < n; j++)
            {
                Vector3 newPos = points[i] + new Vector3(xStep * j, 0, zStep * j);
                //Need to fix rotation
                CreateBuilding(newPos, Quaternion.identity, scale , parent);
            }


        }

    }



    void CreateBuilding(Vector3 position, Quaternion rotation, float scale , GameObject parent)
    {
        GameObject building = GetRandomBuilding();
        GameObject newBuilding = Instantiate(building, parent.transform);
        //Instantiate(building, position, rotation);
        newBuilding.transform.localRotation = rotation;
        newBuilding.transform.localPosition = position;

        newBuilding.transform.localScale = new Vector3(scale, scale, scale);
    }

    void CreateTree(Vector3 position, Quaternion rotation, float scale, GameObject parent)
    {
        GameObject tree = GetRandomTree();
        GameObject newTree = Instantiate(tree,parent.transform);
        newTree.transform.localRotation = rotation;
        newTree.transform.localPosition = position;

        newTree.transform.localScale = new Vector3(scale, scale, scale);
    }

    GameObject GetRandomBuilding()
    {
        return buildings[Random.Range(0, buildings.Length)];
    }

    GameObject GetRandomTree()
    {
        return trees[Random.Range(0, trees.Length)];
    }
}
