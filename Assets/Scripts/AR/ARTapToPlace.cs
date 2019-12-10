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
    public GameObject sceneInstance;
    public ARCamScale camScaleScript;


    private ARTapToPlace arTap;
    public GameObject spawnObject;
    

    //AR managers
    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycast;
    private ARPlaneManager arPlanes;
    private ARPointCloudManager arCloud;
    private ARTrackedImageManager arImageManager;
    private ARCameraManager arCameraManager;

    //Placement
    private ARPlane placementPlane;
    private Pose placementPose;
    private Mesh indMesh;
    private List<Vector3> boundaries;
    private bool placementPoseIsValid = false;
    private float scale;
    private MapCreator mapCreator;
    private List<Vector3> spawnPoints;

    private Dictionary<TrackableId, GameObject> spawnedCans;


    GameObject resetButton;
    GameObject placeButton;

    //setup scene?
    private bool init = true;
    
    // Start is called before the first frame update
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep; //disable screen dimming on Mobile Devices. We might want to change so that it dims while in the menu. 
        Application.targetFrameRate = 60; //Set framerate to 60hz. This improves plane detection in my experience. 
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycast = arOrigin.GetComponent<ARRaycastManager>();
        arPlanes = arOrigin.GetComponent<ARPlaneManager>();
        arCloud = arOrigin.GetComponent<ARPointCloudManager>();
        arCameraManager = Camera.main.GetComponent<ARCameraManager>();


        indMesh = placementIndicator.GetComponentInChildren<MeshFilter>().mesh;
        
        arImageManager = arOrigin.GetComponent<ARTrackedImageManager>();
        mapCreator = GetComponentInChildren<MapCreator>();

        spawnedCans = new Dictionary<TrackableId, GameObject>();
        spawnPoints = new List<Vector3>();

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
            UpdateTrashCanPosition();
        }
        
  
    }

    void UpdateTrashCanPosition()
    {
        //Raycasts to images to get their position. 
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        arRaycast.Raycast(screenCenter, hits, TrackableType.Image);

        if (hits.Count> 0)
        {
            Debug.Log(hits[0].pose.position.ToString());
            if (!spawnedCans.ContainsKey(hits[0].trackableId)){
                GameObject s = Instantiate(spawnObject, hits[0].pose.position, hits[0].pose.rotation);
                
                spawnedCans.Add(hits[0].trackableId, s);
            }
            else
            {
                
                GameObject s = spawnedCans[hits[0].trackableId];
                if (hits[0].pose.position.magnitude - s.transform.position.magnitude > 0.02f)
                {
                    s.transform.position = hits[0].pose.position;
                }
                    
            }
     
        }
        
    }

    public void Restart()
    {
        placeButton.SetActive(true);
        resetButton.SetActive(false);
        arCameraManager.focusMode = CameraFocusMode.Auto;
        mapCreator.RemoveMap();

        Destroy(sceneInstance);
        arPlanes.enabled = true;
        arCloud.enabled = true;
        init = true;
        Camera.main.GetComponent<CamScript>().enabled = false;
        camScaleScript.Disable();
    }


    public void PlaceScene()
    {

        //Create the level at this point in space. 
        if (placementPoseIsValid && sceneInstance == null)
        {
            //Instantiate the prefab
            sceneInstance = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
            //sceneInstance.transform.localScale = new Vector3(scale*0.012f, scale*0.012f, scale*0.012f);
   
            camScaleScript.SetSurfaceScale(scale);
            camScaleScript.SetTarget(sceneInstance.transform);
            camScaleScript.Enable();


            placementPlane.transform.localScale = new Vector3(scale*1000, scale * 1000, scale * 1000); //needs to scale up as camscalescript scales everything down 
            placementPlane.transform.Translate(new Vector3(0.0f, -0.1f, 0.0f));
            mapCreator.CreateMap(sceneInstance);

            foreach(GameObject g in spawnedCans.Values)
            {
                float angle = placementPose.rotation.eulerAngles.y;
                g.transform.RotateAround(sceneInstance.transform.position, new Vector3(0.0f, 1.0f, 0.0f), -angle);
                Vector3 position = (g.transform.position-sceneInstance.transform.position)*(1/(scale*0.012f));
                position.y = 0.0f;
                
                spawnPoints.Add(position);
               
                Destroy(g);
            }

            if(spawnPoints.Count== 0)
            {
                //Add atleast one trash can
                spawnPoints.Add(new Vector3(0, 0, 0));
            }
            

            //create trashcans
            mapCreator.CreateMapTrashCans(spawnPoints.ToArray(), sceneInstance);
            

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

            arCameraManager.focusMode = CameraFocusMode.Fixed;
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

            Vector3[] verts = indMesh.vertices;
            boundaries = new List<Vector3>();

            placementIndicator.transform.localScale = new Vector3(100, 100, 100);
            for (int k = 0; k<verts.Length; k++)
            {
                
                for (int i = 0; i < placementPlane.boundary.Length; i++)
                {

                    if (i > 0)
                    {

                        //Find 
                        Vector3 intersect;
                        Vector3 bound = placementPlane.transform.TransformPoint(placementPlane.boundary[i]);
                        Vector3 edge = bound - placementPlane.transform.TransformPoint(placementPlane.boundary[i - 1]);
                        Vector3 center = placementPlane.center;
                        Vector3 line = verts[k] - center;
                    
                        bool hit = Math3d.LineLineIntersection(out intersect, bound, edge, center, line);

                        if(hit == true)
                        {
                            Debug.Log(hit.ToString());
                            if (Vector3.Distance(placementPlane.center, intersect) < Vector3.Distance(placementPlane.center, verts[k]))
                            {
                                verts[k] = intersect;
                            }
                        }
                       
                      
                   


                    }


                }
            }


            for (int k = 0; k < verts.Length; k++)
            {
                Debug.Log(verts[k].ToString("F4"));
            }
                //Debug.Log(boundaries.Count.ToString());

                indMesh.vertices = verts;
            indMesh.RecalculateNormals();
            
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        if (Camera.current != null)
        {
            var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            var hits = new List<ARRaycastHit>();

            arRaycast.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds);

            placementPoseIsValid = hits.Count > 0;
            if (placementPoseIsValid)
            {
                var trackId = hits[0].trackableId;

                placementPlane = arPlanes.GetPlane(trackId);

                placementPose.position = placementPlane.center;


                //Set rotation of indicator to follow phones forward. 
                var cameraForward = Camera.current.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(placementPlane.normal);//cameraBearing); //Maybe rotated according to the planes normal? 
            }
        }
    }
}
