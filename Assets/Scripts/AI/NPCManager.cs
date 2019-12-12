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
        public int trashIndex;
        public SpawnInfo(Transform spawnPoint, Transform closestTrashCan, int trashIndex)
        {
            this.spawnPoint = spawnPoint;
            this.closestTrashCan = closestTrashCan;
            this.trashIndex = trashIndex;
        }
    }

    [SerializeField]
    private GameObject npcPrefab;
     
    internal Transform parentObject;
    private Transform[] spawnPoints;
    internal Transform[] trashCans;
    internal bool[] fullTrashCans;
    // Switch trash can model after each value on this array.
    private int[] trashCanThresholds = { 7, 15, maxTrashAmount};
    // For each trash can, hold the index of the current trash can model, this is the index if the child object that is active.
    private int[] currentTrashCanModel;
    private int amountOfFullTrashCans = 0;
    private SpawnInfo[] spawnInfos;
    private bool hasStarted = false;

    // Wave control
    private const int amountOfWaves = 10;
    private int amountOfTrashOnGround = 0;
    private int[] amountOfTrashInTrashCan;
    internal bool gameOver = false;
    private const int maxTrashAmount = 20;

    public void SetUp(Transform[] spawnPoints, Transform[] trashCans, Transform parentObject)
    {
        if(!hasStarted)
        {
            hasStarted = true;
            // Get parent object object
            this.parentObject = parentObject;
            
            this.spawnPoints = spawnPoints;
            this.trashCans = trashCans;
            amountOfTrashInTrashCan = new int[trashCans.Length];
            spawnInfos = new SpawnInfo[spawnPoints.Length];
            fullTrashCans = new bool[trashCans.Length];
            currentTrashCanModel = new int[trashCans.Length];
            GetClosestTrashCans();
            StartCoroutine(SpawnWaves());
        }
    }

    private IEnumerator SpawnWaves()
    {
        float timeBetweenWaves = 4f;
        float timeBetweenNPCs = 2f;
        const float timeBetweenWaves_Min = 2f;
        const float timeBetweenNPCs_Min = 0.2f;
        const float timeBetweenWaves_Decrement = 0.5f;
        const float timeBetweenNPCs_Decrement = 0.2f;
        for (;;)
        {
            yield return StartCoroutine(SpawnNPCs(timeBetweenNPCs));
            yield return new WaitForSeconds(timeBetweenWaves);

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
                npc.GetComponent<NPCController>().ActivateAgent(this, spawnInfo.trashIndex);
                yield return new WaitForSeconds(waitTimeBetweenNPCs);
            }
        }
    }

    private void EndGame()
    {
        gameOver = true;
        StopAllCoroutines();
    }

    private void GetClosestTrashCans()
    {
        Transform spawnPoint;
        Transform closest;
        int trashIndex;
        float closestDistanceSqr;
        Transform trashCan;
        float distSqr;

        for (int i = 0; i < spawnPoints.Length; i++) 
        {
            spawnPoint = spawnPoints[i];
            closest = null;
            trashIndex = 0;
            closestDistanceSqr = 1000000f;

            for(int j = 0; j < trashCans.Length; j++){
                if(!fullTrashCans[j])
                {
                    trashCan = trashCans[j];
                    distSqr = Vector3.SqrMagnitude(spawnPoint.position - trashCan.position);
                    if (distSqr < closestDistanceSqr)
                    {
                        closest = trashCan;
                        closestDistanceSqr = distSqr;
                        trashIndex = j;
                    }
                }
            }
            if(closest == null || gameOver)
            {
                return;
            }
            spawnInfos[i] = new SpawnInfo(spawnPoint, closest, trashIndex);
        }
    }

    public int GetClosestTrashCanForAgent(Vector3 currentPos)
    {
        int nextIndex = -1;
        float closestDistanceSqr = int.MaxValue;
        float distSqr;
        if (!gameOver)
        {
            for (int i = 0; i < trashCans.Length; i++)
            {
                if (fullTrashCans[i])
                    continue;
                distSqr = Vector3.SqrMagnitude(currentPos - trashCans[i].position);
                if(distSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distSqr;
                    nextIndex = i;
                }
                    
            }
        }
        return nextIndex; 
    }

    public int GetTrashOnGround()
    {
        return amountOfTrashOnGround;
    }

    public void AddTrashOnGround() {
        amountOfTrashOnGround++;
    }

    public int[] GetTrashInTrashCans()
    {
        return amountOfTrashInTrashCan;
    }

    public void AddTrashInTrashCan(int index)
    {
        if(!gameOver && !fullTrashCans[index])
        {
            amountOfTrashInTrashCan[index]++;
            //TODO: till Tom, anropa funtion här!

            // Check if trash can model needs to change.
            SwitchTrashCanModel(index);
            // This trash can is now full!
            if (amountOfTrashInTrashCan[index] >= maxTrashAmount)
            {
                Debug.Log("Nr " + index + " is full!");
                fullTrashCans[index] = true;
                amountOfFullTrashCans++;
                // Check if all trash cans are full. 
                if(amountOfFullTrashCans >= fullTrashCans.Length)
                    EndGame();
                else
                    // Recalculate the closest trash cans, not including the full ones. 
                    GetClosestTrashCans();
            }
        }
    }

    /**
     * Check if trash can model should be changed and then change it.
     */
    private void SwitchTrashCanModel(int trashIndex)
    {
        // This is the index of the trash can model that is currently active
        int i = currentTrashCanModel[trashIndex];
        if (i < trashCanThresholds.Length && amountOfTrashInTrashCan[trashIndex] == trashCanThresholds[i])
        {
            trashCans[trashIndex].GetChild(i).gameObject.SetActive(false);
            trashCans[trashIndex].GetChild(i+1).gameObject.SetActive(true);
            currentTrashCanModel[trashIndex] = i + 1;
        }
    }

    public void Stop()
    {
        gameOver = true;
        StopAllCoroutines();
    }

}
