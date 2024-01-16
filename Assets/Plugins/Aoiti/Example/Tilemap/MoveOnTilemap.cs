using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;
using System.Linq;
using TEngine.Core;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class MoveOnTilemap : MonoBehaviour
{
    Vector3Int[] directions=new Vector3Int[4] {Vector3Int.left,Vector3Int.right,Vector3Int.up,Vector3Int.down };

    public Tilemap tilemap;
    public TileAndMovementCost[] tiles;
    Pathfinder<Vector3Int> pathfinder;

    [System.Serializable]
    public struct TileAndMovementCost
    {
        public Tile tile;
        public bool movable;
        public float movementCost;
    }

    public List<Vector3Int> path;
    [Range(0.001f,1f)]
    public float stepTime;


    public float DistanceFunc(Vector3Int a, Vector3Int b)
    {
        return (a-b).sqrMagnitude;
    }


    public Dictionary<Vector3Int,float> connectionsAndCosts(Vector3Int a)
    {
        Dictionary<Vector3Int, float> result= new Dictionary<Vector3Int, float>();
        foreach (Vector3Int dir in directions)
        {
            foreach (TileAndMovementCost tmc in tiles)
            {
                if (tilemap.GetTile(a+dir)==tmc.tile)
                {
                    if (tmc.movable) result.Add(a + dir, tmc.movementCost);

                }
            }
                
        }
        return result;
    }

    public CreateGrid m_CreateGrid;
    public Transform End;
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, connectionsAndCosts);
        //这里是瓦片数据测试
        //IEnumerable<GetTileData> empty = tilemap.GetAllTiles();
        //var QQ=empty.ToList();
        //Debug.Log($"全部数量:{QQ.Count}");
        //for (int i = 0; i < QQ.Count; i++)
        //{
        //    //Debug.Log($"瓦片:{QQ[i].Sprite.name}");
        //    Vector3Int vector3In = new Vector3Int(QQ[i].X, QQ[i].Y, 0);
        //    Debug.Log($"第:{i+1}块瓦片位置{vector3In}");
            
        //}

        int MXMin = (int)tilemap.localBounds.min.x;
        int MYMin= (int)tilemap.localBounds.min.y;
        int MXMax = (int)tilemap.localBounds.max.x;
        int MYMax = (int)tilemap.localBounds.max.y;
        Vector3Int vector3IntQ = new Vector3Int(0, 0, 0);
        Debug.Log($"边界MIX:{tilemap.cellBounds.min},MAX:{tilemap.cellBounds.max}");
        
        //for (int i = MXMin; i < MXMax; i++)
        //{
        //    for (int j = MYMin; j < MYMax; j++)
        //    {
        //        Vector3Int vector3Int = new Vector3Int(i,j,0);
        //        Debug.Log($"位于{vector3Int}/*的精灵是:");
        //        if (tilemap.GetSprite(vector3Int)!=null)
        //        {
        //            Debug.Log($":-------------{tilemap.GetSprite(vector3Int).name}");
        //        }

        //    }
        //}


        //Debug.Log($"使用的不同瓦片总数:{tilemap.cellBounds.},精灵数量:{tilemap.GetUsedSpritesCount()}");


    }
    public Vector3 StartPos;
    public Vector3 EndPos;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) )
        {
            m_CreateGrid.FindPath(StartPos, EndPos);
            var currentCellPos=tilemap.WorldToCell(transform.position);
            var target = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            target.z = 0;
            Debug.Log($"鼠标位置:{currentCellPos},目标地块:{target}");
            //pathfinder.GenerateAstarPath(currentCellPos, target, out path);
            GetPathList(currentCellPos, target, out path);
            //Debug.Log("路线数量:"+ path.Count);
            StopAllCoroutines();
            StartCoroutine(Move());
        }

        
    }
    bool GetPathList(Vector3Int StartPos,Vector3Int EndPos,out List<Vector3Int> path)
    {
        List<Vector3Int> vector3IntLs = new List<Vector3Int>();
        List<WorldTile> worldTiles= m_CreateGrid.FindPath(StartPos, EndPos);
        for (int i = 0; i < worldTiles.Count; i++)
        {
            var Data = worldTiles[i];
            Vector3Int vector = new Vector3Int(Data.cellX, Data.cellY);
            vector3IntLs.Add(vector);
            Debug.Log($"第{i + 1}个位置是--X:{worldTiles[i].cellX},Y:{worldTiles[i].cellY}");
        }
        path = vector3IntLs;
        return true;
    }
    Vector3 MovePass = new Vector3(0.5F,0.5F);
    IEnumerator Move()
    {
        while (path.Count > 0)
        {
            transform.position = tilemap.CellToWorld(path[0])+ MovePass;
            path.RemoveAt(0);
            yield return new WaitForSeconds(stepTime);
            
        }
        

    }



    
}

