using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject decal;
    private SphereCollider collider;
    private Rigidbody rb;
    private RaycastHit hit;

    void Awake()
    {
        Destroy(gameObject, 10f);
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, rb.velocity.normalized, out hit, collider.radius + 0.1f))
        {
            Quaternion decalRotation = Quaternion.LookRotation(hit.normal);
            decalRotation *= Quaternion.Euler(0, 180f, 0);
            Instantiate(decal, hit.point, decalRotation);
            print(hit.transform.gameObject.name);
            Renderer otherRenderer = hit.transform.gameObject.GetComponent<Renderer>();
            otherRenderer.material.SetColor("_Color", Color.red);
            Destroy(gameObject);
        }
    }

}
