using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject decal;
    private SphereCollider collider;
    private Rigidbody rb;
    private RaycastHit hit;
    private Color projColor;
    private Vector3 oldPos;
    private bool hasHit;

    [SerializeField]
    private float rayLength = 0.1f;
    [SerializeField]
    private float gravityMultiplier = 0.0005f;

    void Awake()
    {
        Destroy(gameObject, 10f);
        oldPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        projColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        gameObject.GetComponent<Renderer>().material.color = projColor;
        hasHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = transform.position - oldPos;
        if (!hasHit && Physics.Raycast(transform.position, diff.normalized, out hit, diff.magnitude))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                hit.collider.gameObject.GetComponentInParent<NPCController>().HitNpc();
            }

            Quaternion decalRotation = Quaternion.LookRotation(hit.normal);
            float randomZRot = Random.Range(0f, 360f);
            decalRotation *= Quaternion.Euler(0f, 180f, randomZRot);

            float surfaceOffset = 0.05f + Random.value * 0.1f;

            GameObject decalClone = Instantiate(decal, hit.point + (hit.normal * surfaceOffset), decalRotation);
            MeshRenderer decalRenderer = decalClone.GetComponent<MeshRenderer>();
            decalRenderer.material.color = projColor;
            decalClone.transform.parent = hit.transform;
            hasHit = true;

            Destroy(gameObject);
            Destroy(this);
        }

        oldPos = transform.position;
    }

    void FixedUpdate() 
    {
        rb.AddForce(-Vector3.up * Physics.gravity.magnitude * gravityMultiplier);
    }
}
