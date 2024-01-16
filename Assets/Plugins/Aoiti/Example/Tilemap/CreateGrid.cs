using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.ObjectModel;
using System;

public class CreateGrid : MonoBehaviour
{
    public Grid gridBase;
    public Tilemap floor;                         // walkable tilemap
    public List<Tilemap> obstacleLayers;   // all layers that contain objects to navigate around
    public GameObject gridNode;            // where the generated tiles will be stored
    public GameObject nodePrefab;          // world tile prefab

    //these are the bounds of where we are searching in the world for tiles, have to use world coords to check for tiles in the tile map
    public int scanStartX = -300, scanStartY = -300, scanFinishX = 300, scanFinishY = 300, gridSizeX, gridSizeY;

    private List<GameObject> unsortedNodes=new List<GameObject>();   // all the nodes in the world
    public GameObject[,] nodes;           // sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps
    private int gridBoundX = 0, gridBoundY = 0;
    void Start()
    {
        gridSizeX = Mathf.Abs(scanStartX) + Mathf.Abs(scanFinishX);
        gridSizeY = Mathf.Abs(scanStartY) + Mathf.Abs(scanFinishY);
        createGrid();
    }
    public List<WorldTile> getNeighbours(int x, int y, int width, int height)
    {
        List<WorldTile> myNeighbours = new List<WorldTile>();

        if (x > 0 && x < width - 1)
        {
            if (y > 0 && y < height - 1)
            {
                if (nodes[x + 1, y] != null)
                {
                    WorldTile wt1 = nodes[x + 1, y].GetComponent<WorldTile>();
                    if (wt1 != null) myNeighbours.Add(wt1);
                }

                if (nodes[x - 1, y] != null)
                {
                    WorldTile wt2 = nodes[x - 1, y].GetComponent<WorldTile>();
                    if (wt2 != null) myNeighbours.Add(wt2);
                }

                if (nodes[x, y + 1] != null)
                {
                    WorldTile wt3 = nodes[x, y + 1].GetComponent<WorldTile>();
                    if (wt3 != null) myNeighbours.Add(wt3);
                }

                if (nodes[x, y - 1] != null)
                {
                    WorldTile wt4 = nodes[x, y - 1].GetComponent<WorldTile>();
                    if (wt4 != null) myNeighbours.Add(wt4);
                }
            }
            else if (y == 0)
            {
                if (nodes[x + 1, y] != null)
                {
                    WorldTile wt1 = nodes[x + 1, y].GetComponent<WorldTile>();
                    if (wt1 != null) myNeighbours.Add(wt1);
                }

                if (nodes[x - 1, y] != null)
                {
                    WorldTile wt2 = nodes[x - 1, y].GetComponent<WorldTile>();
                    if (wt2 != null) myNeighbours.Add(wt2);
                }

                if (nodes[x, y + 1] == null)
                {
                    WorldTile wt3 = nodes[x, y + 1].GetComponent<WorldTile>();
                    if (wt3 != null) myNeighbours.Add(wt3);
                }
            }
            else if (y == height - 1)
            {
                if (nodes[x, y - 1] != null)
                {
                    WorldTile wt4 = nodes[x, y - 1].GetComponent<WorldTile>();
                    if (wt4 != null) myNeighbours.Add(wt4);
                }
                if (nodes[x + 1, y] != null)
                {
                    WorldTile wt1 = nodes[x + 1, y].GetComponent<WorldTile>();
                    if (wt1 != null) myNeighbours.Add(wt1);
                }

                if (nodes[x - 1, y] != null)
                {
                    WorldTile wt2 = nodes[x - 1, y].GetComponent<WorldTile>();
                    if (wt2 != null) myNeighbours.Add(wt2);
                }
            }
        }
        else if (x == 0)
        {
            if (y > 0 && y < height - 1)
            {
                if (nodes[x + 1, y] != null)
                {
                    WorldTile wt1 = nodes[x + 1, y].GetComponent<WorldTile>();
                    if (wt1 != null) myNeighbours.Add(wt1);
                }

                if (nodes[x, y - 1] != null)
                {
                    WorldTile wt4 = nodes[x, y - 1].GetComponent<WorldTile>();
                    if (wt4 != null) myNeighbours.Add(wt4);
                }

                if (nodes[x, y + 1] != null)
                {
                    WorldTile wt3 = nodes[x, y + 1].GetComponent<WorldTile>();
                    if (wt3 != null) myNeighbours.Add(wt3);
                }
            }
            else if (y == 0)
            {
                if (nodes[x + 1, y] != null)
                {
                    WorldTile wt1 = nodes[x + 1, y].GetComponent<WorldTile>();
                    if (wt1 != null) myNeighbours.Add(wt1);
                }

                if (nodes[x, y + 1] != null)
                {
                    WorldTile wt3 = nodes[x, y + 1].GetComponent<WorldTile>();
                    if (wt3 != null) myNeighbours.Add(wt3);
                }
            }
            else if (y == height - 1)
            {
                if (nodes[x + 1, y] != null)
                {
                    WorldTile wt1 = nodes[x + 1, y].GetComponent<WorldTile>();
                    if (wt1 != null) myNeighbours.Add(wt1);
                }

                if (nodes[x, y - 1] != null)
                {
                    WorldTile wt4 = nodes[x, y - 1].GetComponent<WorldTile>();
                    if (wt4 != null) myNeighbours.Add(wt4);
                }
            }
        }
        else if (x == width - 1)
        {
            if (y > 0 && y < height - 1)
            {
                if (nodes[x - 1, y] != null)
                {
                    WorldTile wt2 = nodes[x - 1, y].GetComponent<WorldTile>();
                    if (wt2 != null) myNeighbours.Add(wt2);
                }

                if (nodes[x, y + 1] != null)
                {
                    WorldTile wt3 = nodes[x, y + 1].GetComponent<WorldTile>();
                    if (wt3 != null) myNeighbours.Add(wt3);
                }

                if (nodes[x, y - 1] != null)
                {
                    WorldTile wt4 = nodes[x, y - 1].GetComponent<WorldTile>();
                    if (wt4 != null) myNeighbours.Add(wt4);
                }
            }
            else if (y == 0)
            {
                if (nodes[x - 1, y] != null)
                {
                    WorldTile wt2 = nodes[x - 1, y].GetComponent<WorldTile>();
                    if (wt2 != null) myNeighbours.Add(wt2);
                }
                if (nodes[x, y + 1] != null)
                {
                    WorldTile wt3 = nodes[x, y + 1].GetComponent<WorldTile>();
                    if (wt3 != null) myNeighbours.Add(wt3);
                }
            }
            else if (y == height - 1)
            {
                if (nodes[x - 1, y] != null)
                {
                    WorldTile wt2 = nodes[x - 1, y].GetComponent<WorldTile>();
                    if (wt2 != null) myNeighbours.Add(wt2);
                }

                if (nodes[x, y - 1] != null)
                {
                    WorldTile wt4 = nodes[x, y - 1].GetComponent<WorldTile>();
                    if (wt4 != null) myNeighbours.Add(wt4);
                }
            }
        }

        return myNeighbours;
    }
    void createGrid()
    {
        int gridX = 0, gridY = 0;
        bool foundTileOnLastPass = false;
        for (int x = scanStartX; x < scanFinishX; x++)
        {
            for (int y = scanStartY; y < scanFinishY; y++)
            {
                TileBase tb = floor.GetTile(new Vector3Int(x, y, 0));
                if (tb != null)
                {
                    bool foundObstacle = false;
                    foreach (Tilemap t in obstacleLayers)
                    {
                        TileBase tb2 = t.GetTile(new Vector3Int(x, y, 0));
                        if (tb2 != null) foundObstacle = true;
                    }

                    Vector3 worldPosition = new Vector3(x + 0.5f + gridBase.transform.position.x, y + 0.5f + gridBase.transform.position.y, 0);
                    GameObject node = (GameObject)Instantiate(nodePrefab, worldPosition, Quaternion.Euler(0, 0, 0));
                    Vector3Int cellPosition = floor.WorldToCell(worldPosition);
                    WorldTile wt = node.GetComponent<WorldTile>();
                    wt.gridX = gridX; wt.gridY = gridY; wt.cellX = cellPosition.x; wt.cellY = cellPosition.y;
                    //node.transform.parent = gridNode.transform;

                    if (!foundObstacle)
                    {
                        foundTileOnLastPass = true;
                        node.name = "Walkable_" + gridX.ToString() + "_" + gridY.ToString();
                        wt.walkable = true;
                    }
                    else
                    {
                        foundTileOnLastPass = true;
                        node.name = "Unwalkable_" + gridX.ToString() + "_" + gridY.ToString();
                        wt.walkable = false;
                        node.GetComponent<SpriteRenderer>().color = Color.red;
                    }

                    unsortedNodes.Add(node);

                    gridY++;
                    if (gridX > gridBoundX)
                        gridBoundX = gridX;

                    if (gridY > gridBoundY)
                        gridBoundY = gridY;
                }
            }

            if (foundTileOnLastPass)
            {
                gridX++;
                gridY = 0;
                foundTileOnLastPass = false;
            }
        }

        nodes = new GameObject[gridBoundX + 1, gridBoundY + 1];

        foreach (GameObject g in unsortedNodes)
        {
            WorldTile wt = g.GetComponent<WorldTile>();
            nodes[wt.gridX, wt.gridY] = g;
        }

        for (int x = 0; x < gridBoundX; x++)
        {
            for (int y = 0; y < gridBoundY; y++)
            {
                if (nodes[x, y] != null)
                {
                    WorldTile wt = nodes[x, y].GetComponent<WorldTile>();
                    wt.myNeighbours = getNeighbours(x, y, gridBoundX, gridBoundY);
                }
            }
        }
    }
    WorldTile GetWorldTileByCellPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = floor.WorldToCell(worldPosition);
        WorldTile wt = null;
        for (int x = 0; x < gridBoundX; x++)
        {
            for (int y = 0; y < gridBoundY; y++)
            {
                if (nodes[x, y] != null)
                {
                    WorldTile _wt = nodes[x, y].GetComponent<WorldTile>();

                    // we are interested in walkable cells only
                    //Debug.Log($"第{x+1}的{y+1}瓦片X:{_wt.cellX},Y:{_wt.cellY},布尔:{_wt.walkable}");
                    if (_wt.walkable && _wt.cellX == cellPosition.x && _wt.cellY == cellPosition.y)
                    {
                        wt = _wt;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        if (wt==null)
        {
            Debug.LogError($"错误是空的瓦片:{worldPosition},X:{cellPosition.x},Y:{cellPosition.y}");
        }
        return wt;
    }
    int GetDistance(WorldTile nodeA, WorldTile nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
    List<WorldTile> RetracePath(WorldTile startNode, WorldTile targetNode)
    {
        List<WorldTile> path = new List<WorldTile>();
        WorldTile currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
    public List<WorldTile> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        WorldTile startNode = GetWorldTileByCellPosition(startPosition);
        if (startNode==null)
        {
            Debug.Log($"FASFASFSAFA");
        }
        WorldTile targetNode = GetWorldTileByCellPosition(endPosition);
        List<WorldTile> openSet = new List<WorldTile>();
        HashSet<WorldTile> closedSet = new HashSet<WorldTile>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Debug.Log($"路线测试");
            WorldTile currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
           // currentNode.GetComponent<SpriteRenderer>().color = Color.blue;
            if (currentNode == targetNode)
            {
                var TTT=RetracePath(startNode, targetNode);
                Debug.Log($"路线数量:{TTT.Count}");
                for (int i = 0; i < TTT.Count; i++)
                {
                    TTT[i].GetComponent<SpriteRenderer>().color = Color.blue;
                }
                return RetracePath(startNode, targetNode);
            }

            foreach (WorldTile neighbour in currentNode.myNeighbours)
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null;
    }
}