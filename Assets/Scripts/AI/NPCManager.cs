using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCManager : MonoBehaviour
{
    private struct SpawnInfo
    {
        public Transform spawnPoint;
        public Transform closestTrashCan;
        public SpawnInfo(Transform spawnPoint, Transform closestTrashCan)
        {
            this.spawnPoint = spawnPoint;
            this.closestTrashCan = closestTrashCan;
        }
    }

    [SerializeField]
    private GameObject npcPrefab;   
     
    private Transform parentObject;
    private Transform[] spawnPoints;
    private Transform[] trashCans;
    private SpawnInfo[] spawnInfos;
    private bool hasStarted = false;
    // Wave control
    private int amountOfWaves = 10;

    public void SetUp(Transform[] spawnPoints, Transform[] trashCans, Transform parentObject)
    {
        if(!hasStarted)
        {
            hasStarted = true;
            // Get parent object object
            //parentObject = GetComponentInParent<TPTP>().t;
            this.parentObject = parentObject;

            this.spawnPoints = spawnPoints;
            this.trashCans = trashCans;
            spawnInfos = new SpawnInfo[spawnPoints.Length];
            GetClosestTrashCans();

            StartCoroutine(SpawnWaves());
        }
    }

    private IEnumerator SpawnWaves()
    {
        float timeBetweenWaves = 4f;
        float timeBetweenNPCs = 2;
        const float timeBetweenWaves_Min = 0.2f;
        const float timeBetweenNPCs_Min = 0.1f;
        const float timeBetweenWaves_Decrement = 1f;
        const float timeBetweenNPCs_Decrement = 1;
        for (;;)
        {
            yield return StartCoroutine(SpawnNPCs(timeBetweenNPCs));
            yield return new WaitForSeconds(timeBetweenWaves);

            //Debug.Log("timeBetweenWaves: " + timeBetweenWaves);
            // This is the time between waves
            if ((timeBetweenWaves - timeBetweenWaves_Decrement) > timeBetweenWaves_Min)
                timeBetweenWaves -= timeBetweenWaves_Decrement;
            else
                timeBetweenWaves = timeBetweenWaves_Min;
            // This is the time between spawning NPCs
            if ((timeBetweenNPCs - timeBetweenNPCs_Decrement) > timeBetweenNPCs_Min)
                timeBetweenNPCs -= timeBetweenNPCs_Decrement;
            else
                timeBetweenNPCs = timeBetweenNPCs_Min;
        }
    }


    private IEnumerator SpawnNPCs(float waitTimeBetweenNPCs)
    {
        if (spawnPoints != null)
        {
            foreach (SpawnInfo spawnInfo in spawnInfos)
            {
                GameObject npc = Instantiate(npcPrefab,parentObject);
                npc.transform.position = spawnInfo.spawnPoint.position;
                npc.GetComponent<NavMeshAgent>().SetDestination(spawnInfo.closestTrashCan.position);
                yield return new WaitForSeconds(waitTimeBetweenNPCs);
            }

        }
    }

    private void GetClosestTrashCans()
    {
        for(int i = 0; i < spawnPoints.Length; i++) 
        {
            Transform spawnPoint = spawnPoints[i];
            Transform closest = trashCans[0];
            float closestDistanceSqr = 1000000f;
            foreach(Transform trashCan in trashCans){
                float distSqr = Vector3.SqrMagnitude(spawnPoint.position - trashCan.position);
                if (distSqr < closestDistanceSqr)
                {
                    closest = trashCan;
                    closestDistanceSqr = distSqr;
                }
            }
            spawnInfos[i] = new SpawnInfo(spawnPoint, closest);
        }
    }

}
