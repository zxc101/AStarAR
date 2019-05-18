using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    private const int COUNT_NODE = 80;

    //public bool onlyDisplayPathGizmos;
    //public List<Node> path;

    private float nodeRadius;
    private float nodeDiameter;
    private float nodeInterval;
    private Vector3 gridWorldSize;
    private int gridSizeX, gridSizeY, gridSizeZ;
    private Node[,,] grid;
    private GameObject[] agents;

    public int MaxSize { get => gridSizeX * gridSizeY * gridSizeZ; }
    public float NodeDiameter { get => nodeDiameter; }
    public float NodeInterval { get => nodeInterval; }
    public LayerMask UnwalkableMask { get => 1 << LayerMask.NameToLayer("Frame"); }

    private void OnValidate()
    {
        //UpdateScaleAndPosition();
    }

    private void UpdateScaleAndPosition()
    {
        agents = GameObject.FindGameObjectsWithTag("Player");
        Vector3 allAgentsPosition = Vector3.zero;
        float[] agentsPositionX = new float[agents.Length];
        float[] agentsPositionY = new float[agents.Length];
        float[] agentsPositionZ = new float[agents.Length];
        for (int i = 0; i < agents.Length; i++)
        {
            agentsPositionX[i] = agents[i].transform.position.x;
            agentsPositionY[i] = agents[i].transform.position.y;
            agentsPositionZ[i] = agents[i].transform.position.z;
            allAgentsPosition += agents[i].transform.position;
        }
        System.Array.Sort(agentsPositionX);
        System.Array.Sort(agentsPositionY);
        System.Array.Sort(agentsPositionZ);
        gridWorldSize.x = (agentsPositionX[agentsPositionX.Length - 1] + 15) - (agentsPositionX[0] - 15);
        gridWorldSize.y = (agentsPositionY[agentsPositionY.Length - 1] + 5) - (agentsPositionY[0] - 5);
        gridWorldSize.z = (agentsPositionZ[agentsPositionZ.Length - 1] + 15) - (agentsPositionZ[0] - 15);
        transform.position = allAgentsPosition / agents.Length;

        if (gridWorldSize.x <= 0)
        {
            gridWorldSize.x = 15;
        }
        if (gridWorldSize.y <= 0)
        {
            gridWorldSize.y = 5;
        }
        if (gridWorldSize.z <= 0)
        {
            gridWorldSize.z = 15;
        }
    }

    /// <summary>
    /// Обновляем данные в Grid
    /// </summary>
    public void GridUpdate()
    {
        UpdateScaleAndPosition();

        nodeRadius = Mathf.Pow((gridWorldSize.x * gridWorldSize.y / 10 * gridWorldSize.z) /
                                Mathf.Pow(COUNT_NODE, 3), 1f / 3);
        nodeDiameter = nodeRadius * 2;
        nodeInterval = nodeRadius / 10;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];

        ///// заполнение массива grid /////

        TrackableHit hit;

        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2 - Vector3.forward * gridWorldSize.z / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius + nodeInterval) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    if(!(Frame.Raycast(worldPoint, Vector3.down, out hit, nodeRadius, raycastFilter)) &&
                       !(Frame.Raycast(worldPoint, Vector3.up, out hit, nodeRadius, raycastFilter)) &&
                       !(Frame.Raycast(worldPoint, Vector3.left, out hit, nodeRadius, raycastFilter)) &&
                       !(Frame.Raycast(worldPoint, Vector3.right, out hit, nodeRadius, raycastFilter)) &&
                       !(Frame.Raycast(worldPoint, Vector3.back, out hit, nodeRadius, raycastFilter)) &&
                       !(Frame.Raycast(worldPoint, Vector3.forward, out hit, nodeRadius, raycastFilter)))
                    {
                        grid[x, y, z] = new Node(true, worldPoint, x, y, z);
                    }
                    else
                    {
                        grid[x, y, z] = new Node(false, worldPoint, x, y, z);
                    }
                }
            }
        }
    }

    private void GridArrayUpdate()
    {

    }

    /// <summary>
    /// Возвращает List из соседних клеток
    /// </summary>
    /// <param name="node">Проверяемая клетка</param>
    /// <returns>Клетки по соседству</returns>
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX &&
                        checkY >= 0 && checkY < gridSizeY &&
                        checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbours.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Находит Node соответствующий заданной позиции
    /// </summary>
    /// <param name="worldPosition">Позиция в мировых координатах</param>
    /// <returns>Node соответствующий заданной позиции</returns>
    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        worldPosition -= transform.position;
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
        
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        return grid[x, y, z];
    }
}
