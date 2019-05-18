using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyController : MonoBehaviour
{
    public GameObject GreenAndroidPrefab;
    public GameObject BlueAndroidPrefab;

    private GameObject prefab;
    void Update()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;

        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;
        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            var andyObject = Instantiate(prefab, hit.Pose.position, Quaternion.identity);
            andyObject.transform.Rotate(0, 180f, 0, Space.Self);
        }
    }

    public void UpdatePrefabToGreen()
    {
        prefab = GreenAndroidPrefab;
    }

    public void UpdatePrefabToBlue()
    {
        prefab = BlueAndroidPrefab;
    }
}
