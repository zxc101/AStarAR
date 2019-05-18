using GoogleARCore;
using System;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public AgentController agentController;
    public TargetController targetController;

    private void OnValidate()
    {
        agentController = transform.parent.Find("AgentController").GetComponent<AgentController>();
        targetController = transform.parent.Find("TargetController").GetComponent<TargetController>();
    }

    private void Update()
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
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                targetController.SpawnTarget(hit.Pose.position);
            }
            else
            {
                agentController.SpawnAgent(hit.Pose.position);
            }
        }
    }
}
