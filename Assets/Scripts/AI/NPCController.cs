using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [SerializeField]
    private float proximity = 3f;
    private int frameSkips = 10;
    private int currentFrame = 0;
    private Vector3 returnPoint;
    private bool hasTrash = true;
    private bool hasReached = false;
    private NavMeshAgent agent;
    [SerializeField]
    private Transform trashHand;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        returnPoint = transform.position;
    }
    void Update()
    {
        // The frame skip thing is added for optimizations
        if(currentFrame <= 0)
        {
            currentFrame = frameSkips;
            if (hasTrash && !hasReached)
            {
                if (Vector3.SqrMagnitude(agent.destination - transform.position) < proximity * proximity)
                {
                    hasTrash = false;
                    hasReached = true;
                    //DropTrash();
                    ThrowTrash();
                    GoHome();
                }
            }
            else
            {
                if (Vector3.SqrMagnitude(returnPoint - transform.position) < proximity * proximity)
                {
                    Destroy(gameObject);
                }
            }
        } else
        {
            currentFrame--;
        }
    }

    private void GoHome()
    {
        agent.SetDestination(returnPoint);
    }

    private void ThrowTrash()
    {
        Destroy(trashHand.gameObject);
    }

    private void DropTrash()
    {
        trashHand.parent = null;
    }
}
