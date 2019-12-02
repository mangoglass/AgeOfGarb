using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


//This code is meant to replace or enhance the default ARImageTracking behavior. I suspect it is not needed. 
public class ARTrackImages : MonoBehaviour
{
    private ARSessionOrigin arOrigin;
    private ARTrackedImageManager arImageManager;
   
    private char[] separator = { '_' };
    // Start is called before the first frame update
    void Awake()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arImageManager = arOrigin.GetComponent<ARTrackedImageManager>();
    }
    void OnEnable()
    { 
        arImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        arImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs ev)
    {
        
        // Check the new tracked images
        foreach (ARTrackedImage image in ev.added)
        {
            XRReferenceImage refImage = image.referenceImage;
            string imgName = refImage.name; // this is the name in the library
            Debug.Log(imgName);
            string[] nameArray = imgName.Split(separator);
            Debug.Log(nameArray);
            string name = nameArray[0];
            int id = Int32.Parse(nameArray[1]);
            if (name == "side")
            {
                //Uppdatera position
                //trashManager.UpdatePosition(id, position);
            }
            if(name == "bottom")
            {
                //töm soptunna
            }
            Debug.Log("____________________________________________________________________________________________________________");
        }
    }

}
