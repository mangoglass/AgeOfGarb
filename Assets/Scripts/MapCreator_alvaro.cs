using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCManager))]
[RequireComponent(typeof(NavMeshSurface))]
public class MapCreator_alvaro : MonoBehaviour
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
    private NavMeshSurface surface;
    private NPCManager npcManager;
    float minDist = 7f;
    float scale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the NavMeshSurface so that we can update it after creating the map.
        surface = GetComponent<NavMeshSurface>();
        npcManager = GetComponent<NPCManager>();
        spawnPoints = new List<Transform>();
        trashCanPositions = new List<Transform>();

        Vector3[] points = new Vector3[]{
            new Vector3(-70,0,-40),
            new Vector3(-70,0,40),
            new Vector3(70,0,40),
            new Vector3(70,0,-40),

        };
        CreateMap(points, gameObject);
        InitializeNPCs();
    }

    private void InitializeNPCs()
    {
        npcManager.SetUp(spawnPoints.ToArray(), trashCanPositions.ToArray());
    }

    void CreateMap(Vector3[] points, GameObject parent)
    {

        CreateBuildingOutline(points, parent);
        CreateTreesInSquare(points[0] + new Vector3(minDist, 0, minDist), points[2] - new Vector3(minDist, 0, minDist), 40, parent);
        CreateGround(points, parent.transform);
        CreateTrashCans(parent.transform);
        // UPDATE NAVMESH
        surface.BuildNavMesh();
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

    //TODO: Rodrigo & Natalie, ni kanske vill utveckla denna funtion. Den tar just nu bara in en papperskorg.
    private void CreateTrashCans(Transform parent)
    {
        if (trashCans.Length > 0)
        {
            GameObject trashCan = Instantiate(trashCans[0], transform);
            trashCan.transform.position = new Vector3(-8f, transform.position.y + 1.26f, 10f);
            trashCan.transform.localScale = new Vector3(3, 3, 3);
            // Viktigt att detta är med varje gång en trashcan skapas
            trashCanPositions.Add(trashCan.transform);
        }

        if (trashCans.Length > 1)
        {
            GameObject trashCan = Instantiate(trashCans[1],transform);
            trashCan.transform.position = new Vector3(12f, transform.position.y + 0.463f, 13f);
            trashCan.transform.localScale = new Vector3(3, 3, 3);
            // Viktigt att detta är med varje gång en trashcan skapas
            trashCanPositions.Add(trashCan.transform);
        }

        if (trashCans.Length > 2)
        {
            GameObject trashCan = Instantiate(trashCans[2], transform);
            trashCan.transform.position = new Vector3(-11f, transform.position.y + 0.463f, -10f);
            trashCan.transform.localScale = new Vector3(3, 3, 3);
            // Viktigt att detta är med varje gång en trashcan skapas
            trashCanPositions.Add(trashCan.transform);
        }

        if (trashCans.Length > 3)
        {
            GameObject trashCan = Instantiate(trashCans[3], transform);
            trashCan.transform.position = new Vector3(18f, transform.position.y + 0.2f, -16f);
            trashCan.transform.localScale = new Vector3(3, 3, 3);
            trashCan.transform.localRotation = Quaternion.Euler(0, -120, 0);
            // Viktigt att detta är med varje gång en trashcan skapas
            trashCanPositions.Add(trashCan.transform);
        }
    }

    void CreateTreesInSquare(Vector3 a, Vector3 b, int n, GameObject parent)
    {
        Vector3 diff = (b - a);

        float xRange = diff.x;
        float zRange = diff.z;

        for (int i = 0; i < n; i++)
        {
            float xf = Random.Range(0f, 1f);
            float zf = Random.Range(0f, 1f);

            Vector3 pos = a + new Vector3(xf * xRange, 0, zf * zRange);
            CreateTree(pos, Quaternion.identity, scale, parent);

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
            //Set building rotation to look towards the yard
            Vector2 yardDir = Vector2.Perpendicular(new Vector2(diff.x, diff.z));
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(yardDir.x,0,yardDir.y), Vector3.up);


            //int n = Mathf.Min(Mathf.FloorToInt(diff.x / minDist), Mathf.FloorToInt(diff.z / minDist));
            int n = Mathf.FloorToInt(diff.magnitude / minDist);

            float xStep = diff.x / n;
            float zStep = diff.z / n;


            for (int j = 0; j < n; j++)
            {
                Vector3 newPos = points[i] + new Vector3(xStep * j, 0, zStep * j);
                //Need to fix rotation
                CreateBuilding(newPos, newRotation, scale, parent);
            }


        }

    }

    void CreateBuilding(Vector3 position, Quaternion rotation, float scale, GameObject parent)
    {
        GameObject building = GetRandomBuilding();
        GameObject newBuilding = Instantiate(building, parent.transform);
        //Instantiate(building, position, rotation);
        newBuilding.transform.localRotation = rotation;
        newBuilding.transform.localPosition = position;

        newBuilding.transform.localScale = new Vector3(scale, scale, scale);

        if (Random.Range(0f, 1f) <= spawnProbability)
        {
            spawnPoints.Add(newBuilding.transform);
        }
    }

    void CreateTree(Vector3 position, Quaternion rotation, float scale, GameObject parent)
    {
        GameObject tree = GetRandomTree();
        GameObject newTree = Instantiate(tree, parent.transform);
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
