using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;
using System.Linq;
using TEngine.Core;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using GabrielBigardi.SpriteAnimator;
using static Sirenix.OdinInspector.Editor.Internal.FastDeepCopier;
using Vector2 = UnityEngine.Vector2;

public class MoveOnTilemap : MonoBehaviour
{
    Vector3Int[] directions=new Vector3Int[4] {Vector3Int.left,Vector3Int.right,Vector3Int.up,Vector3Int.down };

    public Tilemap tilemap;
    public Tilemap Blocktilemap;
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

    public Transform Player;
    SpriteAnimator Animator;
    // Start is called before the first frame update
    void Start()
    {
        Animator = Player.GetComponent<SpriteAnimator>();
        int WalkMapCount = tilemap.GetUsedTilesCount();
        int BlockMapCount= Blocktilemap.GetUsedTilesCount();
        int Count = tilemap.GetUsedTilesCount()+ Blocktilemap.GetUsedTilesCount();
        tiles = new TileAndMovementCost[Count];
        Dictionary<string, Tile> TempDict = new Dictionary<string, Tile>();
        //这里是瓦片数据测试
        IEnumerable<GetTileData> empty = tilemap.GetAllTiles();
        var QQ = empty.ToList();

        Debug.Log($"全部数量:{QQ.Count}");
        for (int i = 0; i < QQ.Count; i++)
        {
            var Data = QQ[i];
            //Debug.Log($"瓦片:{QQ[i].Sprite.name}");
            Vector3Int vector3In = new Vector3Int(Data.X, Data.Y, 0);
           
           // Debug.Log($"第:{i + 1}块瓦片位置{vector3In},瓦片名称:{Data.Tile.name}");
            if (!TempDict.ContainsKey(Data.Tile.name))
            {
                TempDict.Add(Data.Tile.name,(Tile)Data.Tile);
            }
            if (TempDict.Count== WalkMapCount)
            {
                break;
            }
        }
        int Index = 0;
        foreach (var item in TempDict.Values)
        {
            tiles[Index].tile = item;
            tiles[Index].movable = true;
            tiles[Index].movementCost = 5;
            Index++;
        }
        TempDict.Clear();
        Index = 0;
        IEnumerable<GetTileData> empty1 = Blocktilemap.GetAllTiles();
        var QQ1 = empty1.ToList();
        Debug.Log($"阻挡层全部数量:{QQ1.Count}");
        for (int i = 0; i < QQ1.Count; i++)
        {
            var Data = QQ1[i];

            //Debug.Log($"瓦片:{QQ[i].Sprite.name}");
            Vector3Int vector3In = new Vector3Int(Data.X, Data.Y, 0);
            tilemap.SetTile(vector3In,null);

            //Debug.Log($"第:{i + 1}块瓦片位置{vector3In},瓦片名称:{Data.Tile.name}");
            if (!TempDict.ContainsKey(Data.Tile.name))
            {
                TempDict.Add(Data.Tile.name, (Tile)Data.Tile);
            }
            //if (TempDict.Count == BlockMapCount)
            //{
            //    break;
            //}
        }
        foreach (var item in TempDict.Values)
        {
            tiles[Index+ WalkMapCount].tile = item;
            Index++;
        }


        pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, connectionsAndCosts);

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
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) )
        {
            var currentCellPos=tilemap.WorldToCell(transform.position);
            var target = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            int DirValue = 0;
            target.z = 0;
            Debug.Log($"鼠标位置:{currentCellPos},目标地块:{target}");
            if (target.x > transform.position.x)
            {
                DirValue = 1;
            }
            else if (target.x < transform.position.x)
            {
                DirValue = -1;
            }
            pathfinder.GenerateAstarPath(currentCellPos, target, out path);
            //GetPathList(currentCellPos, target, out path);
            //Debug.Log("路线数量:"+ path.Count);
            StopAllCoroutines();
            if (path.Count > 0)
            {
                Animator.Play("Move");
                movePoint = tilemap.CellToWorld(path[0]) + MovePass;
                if (transform.position.x > movePoint.x)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else if (transform.position.x < movePoint.x)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                StartCoroutine(Move());
            }
            else
            {
                if (!Animator.AnimationPlaying("Idle"))
                {
                    Animator.Play("Idle");
                }
                
            }
        }

        
    }
    Vector3 MovePass = new Vector3(0.5F,0.5F);
    public float Acceleration;
    Vector2 movePoint;
    IEnumerator Move()
    {
        while (path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position,movePoint, Acceleration * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, movePoint) <= .001f)
            {
                path.RemoveAt(0);
                if (path.Count > 0)
                {
                    movePoint = tilemap.CellToWorld(path[0]) + MovePass;
                    if (transform.position.x> movePoint.x)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                    }else if (transform.position.x < movePoint.x)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    Animator.Play("Idle");
                }    
            }
            //transform.position = tilemap.CellToWorld(path[0])+ MovePass;
            //path.RemoveAt(0);
            yield return new WaitForEndOfFrame();
            
        }
    }



    
}

