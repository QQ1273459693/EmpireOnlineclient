using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;
using DG.Tweening;
using GabrielBigardi.SpriteAnimator;

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

    ///<summary>开始寻路触发事件</summary>
    public event System.Action OnNavigationStarted;
    ///<summary>到达寻路终点事件</summary>
    public event System.Action OnDestinationReached;
    ///<summary>寻路中改变寻路路径事件</summary>
    public event System.Action<Vector2> OnNavigationChangePoint;



    public SpriteAnimator animator;
    Vector2 m_CurClickMousPos;
    public Transform PlayerTrs;

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

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, connectionsAndCosts);


        OnNavigationStarted += OnStartNavigation;
        OnDestinationReached += OnEndNavigation;
        OnNavigationChangePoint += OnChangeNavigation;
    }
    /// <summary>
    /// 开始寻路
    /// </summary>
    void OnStartNavigation()
    {
        bool CanPlay = false;
        if (m_CurClickMousPos.x > transform.position.x && PlayerTrs.localRotation.y == 1)
        {
            CanPlay = true;
        }
        else if (m_CurClickMousPos.x < transform.position.x && PlayerTrs.localRotation.y == 0)
        {
            CanPlay = true;
        }
        if (CanPlay)
        {

            animator.PlayIfNotPlaying("Trun").OnComplete(() =>
            {
                if (m_CurClickMousPos.x > transform.position.x)
                {
                    PlayerTrs.localRotation = new Quaternion(0, 0, 0, 0);
                }
                else if (m_CurClickMousPos.x < transform.position.x)
                {
                    PlayerTrs.localRotation = new Quaternion(0, 180F, 0, 0);
                }


                if (!animator.AnimationPlaying("Moveing"))
                {
                    //如果不是在移动中
                    animator.Play("Moveing");
                }
            });
        }
        else
        {
            if (!animator.AnimationPlaying("Moveing"))
            {
                animator.Play("BegingMove").OnComplete(() =>
                {
                    animator.Play("Moveing");
                });
                //如果不是在移动中

            }
        }
    }
    /// <summary>
    /// 到达寻路位置
    /// </summary>
    void OnEndNavigation()
    {
        animator.Play("MoveEnd").OnComplete(() =>
        {
            animator.Play("Idle");
        });
        Debug.Log("Click Destination Reached");
    }
    /// <summary>
    /// 寻路中改变寻路路径
    /// </summary>
    /// <param name="pos"></param>
    void OnChangeNavigation(Vector2 pos)
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) )
        {
            var currentCellPos=tilemap.WorldToCell(transform.position);
            m_CurClickMousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var target = tilemap.WorldToCell(m_CurClickMousPos);
            target.z = 0;
            pathfinder.GenerateAstarPath(currentCellPos, target, out path);
            OnNavigationStarted();
            MoveToTarget();
            //StopAllCoroutines();
            //StartCoroutine(Move());
            Debug.Log("看下移动位置:" );
        }

        
    }
    void MoveToTarget()
    {
        if (path.Count>0)
        {
            //Vector2 vector2 = new Vector2(path[0].x/2, path[0].y/2);
            Debug.Log("看下移动位置:"+ path[0]);
            transform.DOMove(path[0], 0.15F).SetEase(Ease.Linear).OnComplete(() =>
            {
                path.RemoveAt(0);
                MoveToTarget();
            });
        }
        else
        {
            OnDestinationReached();
        }
    }

    IEnumerator Move()
    {
        while (path.Count > 0)
        {
            transform.position = tilemap.CellToWorld(path[0]);
            path.RemoveAt(0);
            yield return new WaitForSeconds(stepTime);
            
        }
        

    }



    
}

