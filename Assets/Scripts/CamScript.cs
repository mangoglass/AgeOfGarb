using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    private Camera cam;
    public GameObject projectileClass;
    [SerializeField]
    private float spawnOffset = 0.1f;
    [SerializeField]
    private float projectileForce = 20f;
   

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                GameObject projectile = Instantiate(projectileClass, cam.transform.position + (cam.transform.forward * spawnOffset), cam.transform.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.AddForce(cam.transform.forward * projectileForce, ForceMode.Impulse);
            }    
        }

        // This part is only used for quick dev testing on PC
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject projectile = Instantiate(projectileClass, cam.transform.position + (cam.transform.forward * spawnOffset), cam.transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce(cam.transform.forward * projectileForce, ForceMode.Impulse);
        }
    }
}
