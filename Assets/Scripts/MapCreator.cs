using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NPCManager))]
//[RequireComponent(typeof(NavMeshSurface))]
public class MapCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] buildings;
    [SerializeField]
    private GameObject[] trees;
    [SerializeField]
    private GameObject groundPrefab;
    [SerializeField]
    private GameObject[] trashCans;
    [Range(0f, 1f)]
    [SerializeField]
    private float spawnProbability = 0.5f;

    private List<Transform> spawnPoints;
    private List<Transform> trashCanPositions;
    //private NavMeshSurface surface;
    //private NPCManager npcManager;
    float minDist = 7f;
    float scale = 1f;

    public GameObject temporary;


    public GameObject grassTile;

    public GameObject[] clouds;

    // Start is called before the first frame update
    void Awake()
    {

        //Vector3[] points = new Vector3[]{
        //    new Vector3(-40,0,-40),
        //    new Vector3(-40,0,40),
        //    new Vector3(40,0,40),
        //    new Vector3(40,0,-40),

        //};
        //CreateMap(points, temporary);


        //surface = GetComponent<NavMeshSurface>();
        //npcManager = GetComponent<NPCManager>();
        spawnPoints = new List<Transform>();
        trashCanPositions = new List<Transform>();


    }


    private void InitializeNPCs()
    {
        //npcManager.SetUp(spawnPoints.ToArray(), trashCanPositions.ToArray());
    }

    public void CreateMap( GameObject parent)
    {
        Vector3[] points = new Vector3[]{
            new Vector3(-40,0,-40),
            new Vector3(-40,0,40),
            new Vector3(40,0,40),
            new Vector3(40,0,-40), };



        //GameObject newGT = Instantiate(grassTile, parent.transform);
        //newGT.transform.localPosition = new Vector3(50f, 0, 50f);
        //newGT.transform.localScale = new Vector3(21f, 1f, 21f);


        CreateBuildingOutline(points, parent);
        CreateTreesInSquare(points[0] + new Vector3(minDist, 0, minDist), points[2] - new Vector3(minDist, 0, minDist), 40, parent);
        CreateCloudsInSquare(points[0] - new Vector3(minDist, 0, minDist), points[2] + new Vector3(minDist, 0, minDist), 10, parent);

        CreateGround(points, parent.transform);
        CreateTrashCans(parent.transform);
        // UPDATE NAVMESH
        //surface.BuildNavMesh();


        InitializeNPCs();

    }

    //TODO: Rodrigo & Natalie, ni kanske vill utveckla denna funtion. Den tar just nu bara in en papperskorg.
    private void CreateTrashCans(Transform parent)
    {
        if (trashCans.Length > 0)
        {
            GameObject trashCan = Instantiate(trashCans[0]);
            trashCan.transform.position = new Vector3(0, transform.position.y + 0.463f, 0);
            trashCan.transform.localScale = new Vector3(3, 3, 3);
            // Viktigt att detta är med varje gång en trashcan skapas
            trashCanPositions.Add(trashCan.transform);
        }
    }

    private void CreateGround(Vector3[] points, Transform parent)
    {
        if (points.Length == 4)
        {
            GameObject ground = Instantiate(groundPrefab, parent);
            Mesh mesh = new Mesh();
            mesh.vertices = points;
            int[] triangles = { 0, 1, 2, 2, 3, 0 };
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            ground.GetComponent<MeshFilter>().mesh = mesh;
            ground.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }


    void CreateCloudsInSquare(Vector3 a, Vector3 b, int n, GameObject parent)
    {
        Vector3 diff = (b - a);
        float xRange = diff.x;
        float zRange = diff.z;

        for (int i = 0; i < n; i++)
        {
            float xf = Random.Range(0f, 1f);
            float zf = Random.Range(0f, 1f);

            Vector3 pos = a + new Vector3(xf * xRange, Random.Range(40,80), zf * zRange);
            //CreateCloud(pos, Quaternion.identity, scale, parent);
            CreateObject(clouds, pos, Quaternion.identity, scale, parent);

        }
    }

    void CreateTreesInSquare(Vector3 a, Vector3 b, int n, GameObject parent)
    {
        Vector3 diff = (b - a);


        //Vector3 newAxisX = points[1] - points[0];
        //Vector3 newAxisZ = points[3] - points[0];


        float xRange = diff.x;
        float zRange = diff.z;

        for (int i = 0; i < n; i++) {
            float xf = Random.Range(0f, 1f);
            float zf = Random.Range(0f, 1f);

            Vector3 pos = a + new Vector3(xf * xRange, 0, zf * zRange);
            //CreateTree(pos, Quaternion.identity, scale,parent);
            CreateObject(trees, pos, Quaternion.identity, scale, parent);

            //Vector3 pos = a + new Vector3(newAxisX * xRange, 0, newAxisZ * zRange);

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
                //CreateBuilding(newPos, Quaternion.identity, scale , parent);
                GameObject newBuilding = CreateObject(buildings,newPos, Quaternion.identity, scale, parent);
                //TODO LÄGG TILL ALVAROS RANDOM SAK HÄR FÖR SPAWNPOINTS
                if (Random.Range(0f, 1f) <= spawnProbability)
                {
                    spawnPoints.Add(newBuilding.transform);
                }
            }


        }

    }



    //void CreateBuilding(Vector3 position, Quaternion rotation, float scale , GameObject parent)
    //{
    //    GameObject building = GetRandomFromList(buildings);
    //    GameObject newBuilding = Instantiate(building, parent.transform);
    //    //Instantiate(building, position, rotation);
    //    newBuilding.transform.localRotation = rotation;
    //    newBuilding.transform.localPosition = position;

    //    newBuilding.transform.localScale = new Vector3(scale, scale, scale);
    //}

    //void CreateTree(Vector3 position, Quaternion rotation, float scale, GameObject parent)
    //{
    //    GameObject tree = GetRandomFromList(trees);
    //    GameObject newTree = Instantiate(tree,parent.transform);
    //    newTree.transform.localRotation = rotation;
    //    newTree.transform.localPosition = position;

    //    newTree.transform.localScale = new Vector3(scale, scale, scale);
    //}

    //void CreateCloud(Vector3 position, Quaternion rotation, float scale, GameObject parent)
    //{
    //    GameObject cloud = GetRandomFromList(clouds);
    //    GameObject newCloud = Instantiate(cloud, parent.transform);
    //    newCloud.transform.localRotation = rotation;
    //    newCloud.transform.localPosition = position;

    //    newCloud.transform.localScale = new Vector3(scale, scale, scale);
    //}


    GameObject CreateObject(GameObject[] objects, Vector3 position, Quaternion rotation, float scale, GameObject parent)
    {
        GameObject myObject = GetRandomFromList(objects);
        GameObject newObject = Instantiate(myObject, parent.transform);
        newObject.transform.localRotation = rotation;
        newObject.transform.localPosition = position;

        newObject.transform.localScale = new Vector3(scale, scale, scale);
        return newObject;
    }

    GameObject GetRandomFromList(GameObject[] objects)
    {
        return objects[Random.Range(0, objects.Length)];
    }
}
