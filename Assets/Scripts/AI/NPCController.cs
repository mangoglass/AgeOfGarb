using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [SerializeField]
    private float proximity = 3f;
    [SerializeField]
    private float returnSpeed = 3f;
    [SerializeField]
    private Transform trashHand;
    [SerializeField]
    private float trashAcceleration = 10f;

    private Rigidbody trash_rb;
    private int frameSkips = 10;
    private int currentFrame = 0;
    private Vector3 returnPoint;
    private bool hasTrash = true;
    private bool hasReached = false;
    private bool isGoingHome = false;

    //DEV:
    private bool once = false;

    private NavMeshAgent agent;


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
                    hasReached = true;
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

            // Check if the agent is stuck or not.
            if (agent.pathStatus == NavMeshPathStatus.PathPartial && !once)
            {
                once = true;
                agent.isStopped = true;
                StartCoroutine(ExplodeNPC());

            }
        } else
        {
            currentFrame--;
        }
    }

    public void HitNpc()
    {
        if(!isGoingHome)
        {
            DropTrash();
            GoHome();
        }
    }

    private void GoHome()
    {
        isGoingHome = true;
        agent.SetDestination(returnPoint);
        agent.speed = returnSpeed;
    }

    private void ThrowTrash()
    {
        hasTrash = false;
        Destroy(trashHand.gameObject);
    }

    private void DropTrash()
    {
        hasTrash = false;
        trashHand.parent = null;
        trashHand.GetComponent<Collider>().enabled = true;
        trash_rb = trashHand.GetComponent<Rigidbody>();
        trash_rb.isKinematic = false;
        trash_rb.AddForce(new Vector3(0, trashAcceleration, 0), ForceMode.VelocityChange);
        trash_rb.AddTorque(new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * trashAcceleration,ForceMode.VelocityChange);
        StartCoroutine(CheckForSpeed());
    }

    private IEnumerator CheckForSpeed()
    {
        //This is basically a timer
        for(int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        // Turn off rigidbody.
        trash_rb.isKinematic = true;
        // Turn off collision.
        trashHand.gameObject.GetComponent<Collider>().enabled = false;
        // Turn on nav mesh obstacle, makes NPC avoid this object instead of walking through it. 
        trashHand.gameObject.GetComponent<NavMeshObstacle>().enabled = true;
    }

    private IEnumerator ExplodeNPC()
    {
        //This is basically a timer
        yield return new WaitForSeconds(3f);

        Transform[] allChilds = new Transform[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            allChilds[i] = transform.GetChild(i);
        }

        foreach(Transform child in allChilds)
        {
            child.parent = null;
            child.gameObject.GetComponent<Collider>().enabled = true;
            if(child.GetComponent<Rigidbody>() == null)
                child.gameObject.AddComponent<Rigidbody>().AddExplosionForce(20f, transform.position, 5f);

            StartCoroutine(KillNPC(child));
        }

        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.AddComponent<Rigidbody>().AddExplosionForce(200f, transform.position, 10f);
        StartCoroutine(KillNPC(transform));
    }

    private IEnumerator KillNPC(Transform transform)
    {
        //This is basically a timer
        yield return new WaitForSeconds(5f);
        Destroy(transform.gameObject);
    }

}
