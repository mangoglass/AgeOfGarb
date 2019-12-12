using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    // Start is called before the first frame update
    float minVal = -60;
    float maxVal = 60;
    float travelTime;
    float speed;

    void Start()
    {
        travelTime = Random.Range(60, 180);
        speed = (maxVal - minVal) / travelTime;
    }

    // Update is called once per frame
    void Update()
    {

        float xPos = gameObject.transform.position.x;
        float newXPos = (xPos + Time.deltaTime * speed);
                if(newXPos > maxVal)
        {
            newXPos -= maxVal - minVal;
        }

        gameObject.transform.position = new Vector3(newXPos, gameObject.transform.position.y, gameObject.transform.position.z);

    }
}
