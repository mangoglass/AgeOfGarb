using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
//using UnityEngine.Experimental.XR;
using System;

public class ARTapToPlace : MonoBehaviour
{

    //Gameobjects
    public GameObject placementIndicator;
    public GameObject objectToPlace;
    private GameObject sceneInstance;


    //AR managers
    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycast;
    private ARPlaneManager arPlanes;
    private ARPointCloudManager arCloud;

    //Placement
    private ARPlane placementPlane;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private float scale;


    GameObject resetButton;
    GameObject placeButton;

    //setup scene?
    private bool init = true;
    
    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep; //disable screen dimming on Mobile Devices. We might want to change so that it dims while in the menu. 
        Application.targetFrameRate = 60; //Set framerate to 60hz. This improves plane detection in my experience. 
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycast = arOrigin.GetComponent<ARRaycastManager>();
        arPlanes = arOrigin.GetComponent<ARPlaneManager>();
        arCloud = arOrigin.GetComponent<ARPointCloudManager>();



        placeButton = GameObject.Find("PlaceScene");
        resetButton = GameObject.Find("Reset");
        resetButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (init)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }
        
  
    }

    public void Restart()
    {



        placeButton.SetActive(true);
        resetButton.SetActive(false);
        Destroy(sceneInstance);
        arPlanes.enabled = true;
        arCloud.enabled = true;
        init = true;
        Camera.main.GetComponent<CamScript>().enabled = false; //
    }


    public void PlaceScene()
    {

        //Create the level at this point in space. 
        if (placementPoseIsValid && sceneInstance == null)
        {
            //Instantiate the prefab
            sceneInstance = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
            sceneInstance.transform.localScale = new Vector3(scale*0.012f, scale*0.012f, scale*0.012f);

            MapCreator mc = GetComponentInChildren<MapCreator>();
            mc.CreateMap(sceneInstance);

            //Disable new plane detection from now on. 
            arPlanes.enabled = false;
            //Remove the debug planes
            foreach (var plane in arPlanes.trackables)
            {
                if (plane != placementPlane)
                {
                    plane.gameObject.SetActive(false);
                }
                    
            }
            //Also disable the point cloud (which is just for fun anyway)
            arCloud.enabled = false;
            foreach (var point in arCloud.trackables)
            {
               point.gameObject.SetActive(false);
            }

            //remove the placementindicator
            placementIndicator.SetActive(false);

           
            init = false;


          
            placeButton.SetActive(false);
            resetButton.SetActive(true);

            //Start the shooting
            Camera.main.GetComponent<CamScript>().enabled = true; //

        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);


            //get the shortest distance to the border of the plane and use that as a scale. 
            Vector2 extents = placementPlane.extents;
            scale = Mathf.Min(new float[] { extents.x, extents.y });
            placementIndicator.transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        arRaycast.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds );

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            var trackId = hits[0].trackableId;

            placementPlane = arPlanes.GetPlane(trackId);

            placementPose.position = placementPlane.center;


            //Set rotation of indicator to follow phones forward. 
            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing); //Maybe rotated according to the planes normal? 
        }
    }
}
