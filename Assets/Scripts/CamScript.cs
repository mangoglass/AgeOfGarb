using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    private Camera cam;
    public GameObject projectileClass;
    [SerializeField]
    private float spawnZOffset = 0.1f;
    [SerializeField]
    private float spawnYOffset = -0.01f;
    [SerializeField]
    private float projectileForce = 0.015f;
    [SerializeField]
    private float shootingTimeOut;
    private float timeOut;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        timeOut = shootingTimeOut;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && timeOut >= shootingTimeOut)
            {
                Vector3 offset = (cam.transform.forward * spawnZOffset) + (cam.transform.up * spawnYOffset);
                GameObject projectile = Instantiate(projectileClass, cam.transform.position + offset, cam.transform.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.AddForce(cam.transform.forward * projectileForce, ForceMode.Impulse);
                AudioManager.instance.Play("Shoot");
                timeOut = 0;
            }
        }

        if (timeOut < shootingTimeOut) timeOut += Time.deltaTime;

        // This part is only used for quick dev testing on PC
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 offset = (cam.transform.forward * spawnZOffset) + (cam.transform.up * spawnYOffset);
            GameObject projectile = Instantiate(projectileClass, cam.transform.position + offset, cam.transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce(cam.transform.forward * projectileForce, ForceMode.Impulse);
            AudioManager.instance.Play("Shoot");
        }
    }
}
