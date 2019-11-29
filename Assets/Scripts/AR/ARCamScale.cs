using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCamScale : MonoBehaviour
{
    [SerializeField]
    private float scale;
    private float surfaceScale;
    private Transform targetTransform;
    private Transform parentTransform;

    void Awake()
    {
        surfaceScale = 1f;
        parentTransform = transform.parent.transform;
        this.enabled = false;
    }

    public void SetSurfaceScale(float surfaceScale)
    {
        this.surfaceScale = surfaceScale;
    }

    public void SetTarget(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public void Enable()
    {
        this.enabled = true;
    }

    public void Disable()
    {
        this.enabled = false;
        surfaceScale = 1f;
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dist = parentTransform.position - targetTransform.position;
        Vector3 cameraPos = dist * (1 / (surfaceScale * scale)) - dist;
        transform.position = cameraPos;
    }
}
