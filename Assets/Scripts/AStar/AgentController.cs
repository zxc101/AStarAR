using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField] private GameObject agent;

    [SerializeField] private float speed = 1;
    [SerializeField] private int countCrossingNodes;

    private AgentStruct currentAgent;
    private GridController gridController;
    private TargetController targetController;
    private RaycastHit hit;

    private bool IsMoveToNextTarget;

    private void OnValidate()
    {
        if (speed < 0)
        {
            speed = 0;
        }
        if(countCrossingNodes < 0)
        {
            countCrossingNodes = 0;
        }
    }

    private void Start()
    {
        gridController = transform.parent.Find("GridController").GetComponent<GridController>();
        targetController = transform.parent.Find("TargetController").GetComponent<TargetController>();
        IsMoveToNextTarget = true;
    }

    private void Update()
    {
        if (IsMoveToNextTarget && targetController.SaveTargets.Count != 0)
        {
            IsMoveToNextTarget = false;
            StartMove();
        }
    }

    public void SpawnAgent(Vector3 position)
    {
        if (currentAgent.prefab == null)
        {
            TrackableHit hit;

            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            currentAgent.prefab = Instantiate(agent, position, Quaternion.identity);
            if (Frame.Raycast(currentAgent.prefab.transform.position, -Vector3.up, out hit, Mathf.Infinity, raycastFilter))
            {
                currentAgent.prefab.transform.position = Vector3.right * currentAgent.prefab.transform.position.x + Vector3.up * (hit.Pose.position.y + currentAgent.prefab.transform.localScale.y * 3 / 4) + Vector3.forward * currentAgent.prefab.transform.position.z;
            }
            currentAgent.speed = speed;

            gridController.GridUpdate();
        }
    }

    public void DestroyAgent()
    {
        if(currentAgent.prefab != null)
        {
            currentAgent.speed = 0;
            currentAgent.path = null;
            Destroy(currentAgent.prefab);
        }
    }

    public void StartMove()
    {
        currentAgent.path = Pathfinder.FindPath(currentAgent.prefab.transform.position, targetController.SaveTargets.Peek().transform.position, countCrossingNodes);
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        int targetIndex = 0;
        Vector3 currentWaypoint = currentAgent.path[targetIndex].worldPosition;

        while (true)
        {
            if (currentAgent.prefab.transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= currentAgent.path.Count)
                {
                    targetController.DestroyTarget();
                    IsMoveToNextTarget = true;
                    yield break;
                }
                currentWaypoint = currentAgent.path[targetIndex].worldPosition;
            }
            currentAgent.prefab.transform.position = Vector3.MoveTowards(currentAgent.prefab.transform.position, currentWaypoint, currentAgent.speed * Time.deltaTime);
            yield return null;
        }
    }
}
