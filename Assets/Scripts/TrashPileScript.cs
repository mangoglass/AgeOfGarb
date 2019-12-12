using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashPileScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] thrashes;
    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = thrashes[Random.Range(0, thrashes.Length)];
        GameObject newObj = Instantiate(obj, gameObject.transform);

        float rot = Random.Range(0, 359);
        newObj.transform.localRotation = Quaternion.Euler(0, rot, 0);

        float sx = Random.Range(0.9f, 1.1f);
        float sy = Random.Range(0.9f, 1.1f);
        float sz = Random.Range(0.9f, 1.1f);
        newObj.transform.localScale = new Vector3(sx, sy, sz);

        rend = newObj.GetComponentInChildren<Renderer>();
        float u = Random.Range(0, 1f);
        float v = Random.Range(0, 1f);
        rend.material.SetTextureOffset("_MainTex", new Vector2(u, v));
    }
}
