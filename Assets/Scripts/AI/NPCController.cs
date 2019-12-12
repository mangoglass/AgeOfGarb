using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCController : MonoBehaviour
{
    [SerializeField]
    private float proximity = 1.5f;
    [SerializeField]
    private float returnSpeed = 3f;
    [SerializeField]
    private Transform flyingTrash;
    [SerializeField]
    internal GameObject animatedTrash;
    [SerializeField]
    private float trashAcceleration = 5f;
    [SerializeField]
    private GameObject[] allCharacters;

    private Rigidbody trash_rb;
    private NPCManager npcManager;
    private Animator animator;
    private bool activateAgent = false;
    private int frameSkips = 8;
    private int currentFrame = 0;
    private Vector3 returnPoint;
    private bool hasTrash = true;
    private bool hasReached = false;
    private bool isGoingHome = false;
    private bool isHit = false;
    private int trashCanIndex;
    private AudioSource[] audioSources;
    public AudioClip[] npcScreamClips;

    internal NavMeshAgent agent;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        returnPoint = transform.position;
        // Randomly chose the character for this npc.
        allCharacters[Random.Range(0, allCharacters.Length - 1)].SetActive(true);

        audioSources = GetComponents<AudioSource>();
        audioSources[0].clip = npcScreamClips[Random.Range(0, npcScreamClips.Length)];
    }

    void Update()
    {
        // Wait for everything to be initialized.
        if(activateAgent)
        {
            if(npcManager.gameOver)
            {
                agent.isStopped = true;
                activateAgent = false;
                animator.SetBool("GameOver", true);
                return;
            }
            // The frame skip thing is added for optimizations
            if (currentFrame <= 0)
            {
                currentFrame = frameSkips;
                if (npcManager.fullTrashCans[trashCanIndex] && !isGoingHome)
                {
                    ChangeTrashCanDestination();
                }

                if (hasTrash && !hasReached)
                {
                    // This is when the npc has reached the trashcan.
                    if (Vector3.SqrMagnitude(agent.destination - transform.position) < proximity * proximity)
                    {
                        animator.SetTrigger("ThrowTrash");
                        ThrowTrash();
                        // OBS! GoHome() is called from a behaviour script called GoHomeBehaviour.cs
                    }
                }
                else
                {
                    // This is when the npc is close to home, he just disappears.
                    if (Vector3.SqrMagnitude(returnPoint - transform.position) < proximity * proximity * 3f)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                currentFrame--;
            }
        }
    }

    public void HitNpc()
    {
        if (!isHit && !npcManager.gameOver)
        {
            isHit = true;
            DropTrash();
        }

        if (!audioSources[0].isPlaying && !npcManager.gameOver)
        {
            audioSources[0].Play();
            animator.SetTrigger("Hit");
        }
    }

    internal void GoHome()
    {
        isGoingHome = true;
        agent.SetDestination(returnPoint);
        agent.speed = returnSpeed;
        agent.avoidancePriority = 99;
        animator.SetBool("GoHome", true);
    }

    // This is called if the npc reaches the trash can. 
    private void ThrowTrash()
    {
        audioSources[1].Play();
        hasReached = true;
        hasTrash = false;
        Destroy(flyingTrash.gameObject);
        npcManager.AddTrashInTrashCan(trashCanIndex);
    }

    // This is called when the player hits an NPC.
    private void DropTrash()
    {
        if(flyingTrash != null)
        {
            flyingTrash.gameObject.SetActive(true);
            hasTrash = false;
            flyingTrash.parent = npcManager.parentObject;
            flyingTrash.GetComponent<Collider>().enabled = true;
            trash_rb = flyingTrash.GetComponent<Rigidbody>();
            trash_rb.isKinematic = false;
            trash_rb.AddForce(new Vector3(Random.Range(-0.3f, 0.3f) * trashAcceleration, trashAcceleration, Random.Range(-0.3f, 0.3f) * trashAcceleration), ForceMode.VelocityChange);
            trash_rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * trashAcceleration, ForceMode.VelocityChange);
            flyingTrash.GetComponent<TrashController>().Begin();
        }
        npcManager.AddTrashOnGround();
        // Remove the animated trash bag
        Destroy(animatedTrash);
    }

    // Call this from the NPCManager script.
    public void ActivateAgent(NPCManager npcManager, int trashCanIndex)
    {
        if(!activateAgent)
        {
            this.npcManager = npcManager;
            this.trashCanIndex = trashCanIndex;
            activateAgent = true;
        }
    }

    // Gives this agent a new destination if the current trash can is full. 
    private void ChangeTrashCanDestination()
    {
        trashCanIndex = npcManager.GetClosestTrashCanForAgent(transform.position);
        if (trashCanIndex == -1)
            return;
        agent.SetDestination(npcManager.trashCans[trashCanIndex].position);
    }
}
