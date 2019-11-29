using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCManager))]
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
    [SerializeField]
    private GameObject surfacePrefab;
    [Range(0f, 1f)]
    [SerializeField]
    private float spawnProbability = 0.5f;

    private List<Transform> spawnPoints;
    private List<Transform> trashCanPositions;
    private NPCManager npcManager;
    float minDist = 7f;
    float scale = 1f;


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

        npcManager = GetComponent<NPCManager>();
        spawnPoints = new List<Transform>();
        trashCanPositions = new List<Transform>();


    }


    private void InitializeNPCs(Transform parentObject)
    {
        npcManager.SetUp(spawnPoints.ToArray(), trashCanPositions.ToArray(),parentObject);
    }

    public void CreateMap( GameObject parent)
    {
        Vector3[] points = new Vector3[]{
            new Vector3(-40,0,-40),
            new Vector3(-40,0,40),
            new Vector3(40,0,40),
            new Vector3(40,0,-40), };

        CreateBuildingOutline(points, parent);
        CreateTreesInSquare(points[0] + new Vector3(minDist, 0, minDist), points[2] - new Vector3(minDist, 0, minDist), 40, parent);
        CreateCloudsInSquare(points[0] - new Vector3(minDist, 0, minDist), points[2] + new Vector3(minDist, 0, minDist), 10, parent);

        CreateGround(points, parent.transform);
        CreateTrashCans(parent.transform);

        // UPDATE NAVMESH
        GameObject surface = Instantiate(surfacePrefab, parent.transform);
        surface.GetComponent<NavMeshSurface>().BuildNavMesh();


        InitializeNPCs(parent.transform);

    }

    public void RemoveMap()
    {
        npcManager.Stop();
    }

    //TODO: Rodrigo & Natalie, ni kanske vill utveckla denna funtion. Den tar just nu bara in en papperskorg.
    private void CreateTrashCans(Transform parent)
    {
        if (trashCans.Length > 0)
        {
            GameObject trashCan = Instantiate(trashCans[0], parent);
            trashCan.transform.localPosition = new Vector3(-8f, parent.position.y + 1.26f, 10f);
            trashCan.transform.localScale = new Vector3(3f, 3f, 3f);
            // Viktigt att detta är med varje gång en trashcan skapas
            trashCanPositions.Add(trashCan.transform);
        }

        if (trashCans.Length > 1)
        {
            GameObject trashCan = Instantiate(trashCans[1], parent);
            trashCan.transform.localPosition = new Vector3(12f, parent.position.y + 0.463f, 13f);
            trashCan.transform.localScale = new Vector3(3f, 3f, 3f);
            // Viktigt att detta är med varje gång en trashcan skapas
            trashCanPositions.Add(trashCan.transform);
        }

        if (trashCans.Length > 2)
        {
            GameObject trashCan = Instantiate(trashCans[2], parent);
            trashCan.transform.localPosition = new Vector3(-11f, parent.position.y + 0.463f, -10f);
            trashCan.transform.localScale = new Vector3(3f, 3f, 3f);
            // Viktigt att detta är med varje gång en trashcan skapas
            trashCanPositions.Add(trashCan.transform);
        }

        if (trashCans.Length > 3)
        {
            GameObject trashCan = Instantiate(trashCans[3], parent);
            trashCan.transform.localPosition = new Vector3(18f, parent.position.y + 0.2f, -16f);
            trashCan.transform.localScale = new Vector3(3f, 3f, 3f);
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

        float n_cells = Mathf.Floor(Mathf.Sqrt(n));
        float x_step = xRange / n_cells;
        float z_step = zRange / n_cells;

        for (int x = 0; x < n_cells; x++)
        {
            for (int z = 0; z < n_cells; z++)
            {
                int n_trees = Mathf.FloorToInt(Mathf.Sqrt(Random.Range(0, 20)));

                for (int i = 0; i < n_trees; i++)
                {
                    float xf = Random.Range(0f, 1f);
                    float zf = Random.Range(0f, 1f);
                    Vector3 pos = a + new Vector3((x + xf) * x_step, 0, (z + zf) * z_step);
                    CreateObject(trees, pos, Quaternion.identity, scale, parent);
                }
            }
        }

        //for (int i = 0; i < n; i++) {
        //    float xf = Random.Range(0f, 1f);
        //    float zf = Random.Range(0f, 1f);

        //    Vector3 pos = a + new Vector3(xf * xRange, 0, zf * zRange);
        //    //CreateTree(pos, Quaternion.identity, scale,parent);
        //    CreateObject(trees, pos, Quaternion.identity, scale, parent);

        //    //Vector3 pos = a + new Vector3(newAxisX * xRange, 0, newAxisZ * zRange);

        //}




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

                Quaternion rot = Quaternion.identity;
                if (Mathf.Abs(diff.x) < Mathf.Abs(diff.z))
                {
                    rot = Quaternion.Euler(0, 90, 0);
                }


                GameObject newBuilding = CreateObject(buildings,newPos, rot, scale, parent);
                newBuilding.transform.localScale = new Vector3(
                    Random.Range(0.9f, 1.2f),
                    Random.Range(0.8f, 1.2f),
                    1);
                //TODO LÄGG TILL ALVAROS RANDOM SAK HÄR FÖR SPAWNPOINTS
                if (Random.Range(0f, 1f) <= spawnProbability)
                {
                    spawnPoints.Add(newBuilding.transform);
                }
            }


        }

    }

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
