using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private static GridController grid;
    private static List<Node> path;

    /// <summary>
    /// Находит путь
    /// </summary>
    /// <param name="startPos">Стартовая позиция</param>
    /// <param name="targetPos">Позиция цели</param>
    public static List<Node> FindPath(Vector3 startPos, Vector3 targetPos, int countCrossingNodes)
    {
        grid = GameObject.Find("GridController").GetComponent<GridController>();
        //grid.GridUpdate();
        path = new List<Node>();
        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos + Vector3.up * (grid.NodeDiameter - grid.NodeInterval));

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode, countCrossingNodes);
                return path;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if(!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        return path;
    }

    private static void RetracePath(Node startNode, Node endNode, int countCrossingNodes)
    {
        Node currentNode = endNode;

        TrackableHit hit;

        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        while (currentNode != startNode)
        {
            // Применяем гравитацию
            Node gravityNode = currentNode;

            if (Frame.Raycast(currentNode.worldPosition, -Vector3.up, out hit, Mathf.Infinity, raycastFilter))
            {
                gravityNode = grid.NodeFromWorldPosition(new Vector3(currentNode.worldPosition.x, hit.Pose.position.y + 0.1f/* + seeker.localScale.y / 2*/, currentNode.worldPosition.z));
            }
            ///////////////////////
            path.Add(gravityNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        //if (path.Count > countCrossingNodes * 3 + 1)
        //{
        //    for (int i = 0; i < path.Count; i++)
        //    {
        //        if (i < path.Count - 1 + countCrossingNodes)
        //        {
        //            if (path[i].worldPosition.y > path[i + 1].worldPosition.y)
        //            {
        //                path.RemoveRange(i + 1, countCrossingNodes);
        //            }
        //        }

        //        if (i > countCrossingNodes)
        //        {
        //            if (path[i].worldPosition.y < path[i + 1].worldPosition.y)
        //            {
        //                path.RemoveRange(i + 1 - countCrossingNodes, countCrossingNodes);
        //            }
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Находит дистанцию между двумя Node-ами
    /// </summary>
    /// <param name="nodeA">Первая Node-а</param>
    /// <param name="nodeB">Вторая Node-а</param>
    /// <returns>Дистанция</returns>
    private static float GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        return Mathf.Sqrt(Mathf.Pow(dstX, 2) + Mathf.Pow(dstY, 2) + Mathf.Pow(dstZ, 2));
    }
}
