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

    void Awake()
    {
        Destroy(gameObject, 10f);
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        projColor = Random.ColorHSV(0, 1, 1, 1, 0, 1);
        gameObject.GetComponent<Renderer>().material.color = projColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, rb.velocity.normalized, out hit, collider.radius + 0.1f))
        {
            Quaternion decalRotation = Quaternion.LookRotation(hit.normal);
            float randomZRot = Random.Range(0f, 360f);
            decalRotation *= Quaternion.Euler(0f, 180f, randomZRot);
            GameObject decalClone = Instantiate(decal, hit.point, decalRotation);
            MeshRenderer decalRenderer = decalClone.GetComponent<MeshRenderer>();
            decalRenderer.material.color = projColor;
            decalClone.transform.parent = hit.transform;
            print(hit.transform.gameObject.name);
            Destroy(gameObject);
        }
    }

}
