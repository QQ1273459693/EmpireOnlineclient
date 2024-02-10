using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aoiti.Pathfinding; //import the pathfinding library 
using static Sirenix.OdinInspector.Editor.Internal.FastDeepCopier;
using UnityEngine.Tilemaps;
using GabrielBigardi.SpriteAnimator;
using TEngine;
using UnityEngine.EventSystems;
using EventSystem = UnityEngine.EventSystems.EventSystem;


//public class NavigationTest: MonoBehaviour
//{
//    Pathfinder<Vector3> pathfinder;
//    List<Vector3> path = new List<Vector3>();

//    private void Start()
//    {
//        pathfinder = new Pathfinder<Vector3>(GetDistance, GetNeighbourNodes);
//    }
//    private void Update()
//    {
//        if (Input.GetMouseButtonDown(0)) //check for a new target
//        {
//            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            var _path = new Vector3[0];
//            if (pathfinder.GenerateAstarPath(transform.position, target, out _path)) //if there is a path from current position to target position reassign path.
//                path = new List<Vector3>(_path); 
//        }

//        transform.position = path[0]; //go to next node
//        path.RemoveAt(0); //remove the node from path

//    }

//    float GetDistance(Vector3 A, Vector3 B)
//    {
//        return (A - B).sqrMagnitude; 
//    }
//    Dictionary<Vector3, float> GetNeighbourNodes(Vector3 pos)
//    {
//        Dictionary<Vector3, float> neighbours = new Dictionary<Vector3, float>();
//        for (int i = -1; i < 2; i++)
//        {
//            for (int j = -1; j < 2; j++)
//            {
//                for (int k=-1;k<2;k++)
//                {

//                    if (i == 0 && j == 0 && k==0) continue;

//                    Vector3 dir = new Vector3(i, j,k);
//                    if (!Physics2D.Linecast(pos, pos + dir))
//                    {
//                        neighbours.Add(pos + dir, dir.magnitude);
//                    }
//                }
//            }

//        }
//        return neighbours;
//    }

//}

public class MovementController2D : MonoBehaviour
{
    [Header("Navigator options")]
    [SerializeField] float gridSize = 0.5f; //increase patience or gridSize for larger maps
    [SerializeField] float speed = 0.05f; //increase for faster movement
    
    Pathfinder<Vector2> pathfinder; //the pathfinder object that stores the methods and patience
    [Tooltip("The layers that the navigator can not pass through.")]
    [SerializeField] LayerMask obstacles;
    [Tooltip("Deactivate to make the navigator move along the grid only, except at the end when it reaches to the target point. This shortens the path but costs extra Physics2D.LineCast")] 
    [SerializeField] bool searchShortcut =false; 
    [Tooltip("Deactivate to make the navigator to stop at the nearest point on the grid.")]
    [SerializeField] bool snapToGrid =false; 
    Vector2 targetNode; //target in 2D space
    List <Vector2> path;
    List<Vector2> pathLeftToGo= new List<Vector2>();
    [SerializeField] bool drawDebugLines;

    // Start is called before the first frame update
    void Start()
    {
        Animator = Player.GetComponent<SpriteAnimator>();
        ClickImg = ClickAroow.GetComponent<SpriteRenderer>();
        pathfinder = new Pathfinder<Vector2>(GetDistance,GetNeighbourNodes,1000); //increase patience or gridSize for larger maps
    }
    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击UI
        EventSystem.current.RaycastAll(eventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            return true;
            Debug.Log("raycastResults[0].gameObject.name=" + raycastResults[0].gameObject.name);
            if (raycastResults[0].gameObject.name == "RawImage")//判断是否 是 自己要点击的UI
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {

            return false;
        }
    }
           // Update is called once per frame
        void Update()
    {
            if (Input.GetMouseButtonDown(0)) //check for a new target
        {
            if (IsPointerOverGameObject(Input.mousePosition))
            {
                return;
            }
            StopAllCoroutines();
            var Target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            GetMoveCommand(Target);
            if (pathLeftToGo.Count > 0)
            {
                ClickImg.color = Color.white;
                ClickAroow.position = pathLeftToGo[pathLeftToGo.Count-1];
                StartCoroutine(ClickShow());
                Animator.Play("Move");
                movePoint = pathLeftToGo[0];
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

        //if (pathLeftToGo.Count > 0) //if the target is not yet reached
        //{
        //    Vector3 dir =  (Vector3)pathLeftToGo[0]-transform.position ;
        //    transform.position += dir.normalized * speed;
        //    if (((Vector2)transform.position - pathLeftToGo[0]).sqrMagnitude <speed*speed) 
        //    {
        //        transform.position = pathLeftToGo[0];
        //        pathLeftToGo.RemoveAt(0);
        //    }
        //}

        if (drawDebugLines)
        {
            for (int i=0;i<pathLeftToGo.Count-1;i++) //visualize your path in the sceneview
            {
                Debug.DrawLine(pathLeftToGo[i], pathLeftToGo[i+1]);
            }
        }
    }
    Vector2 movePoint;
    public Transform Player;
    SpriteAnimator Animator;
    public Transform ClickAroow;
    SpriteRenderer ClickImg;
    IEnumerator Move()
    {
        while (pathLeftToGo.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint, speed * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, movePoint) <= .001f)
            {
                pathLeftToGo.RemoveAt(0);
                if (pathLeftToGo.Count > 0)
                {
                    movePoint = pathLeftToGo[0];
                    if (transform.position.x > movePoint.x)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                    else if (transform.position.x < movePoint.x)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    GameModule.UI.ShowUI<TipsWnd_NPCMessage,int>(1);
                    Animator.Play("Idle");
                }
            }
            //transform.position = tilemap.CellToWorld(path[0])+ MovePass;
            //path.RemoveAt(0);
            yield return null;

        }
    }
    IEnumerator ClickShow()
    {
        yield return new WaitForSeconds(0.45F);
        ClickImg.color = Color.clear;
    }

    void GetMoveCommand(Vector2 target)
    {
        Vector2 closestNode = GetClosestNode(transform.position);
        if (pathfinder.GenerateAstarPath(closestNode, GetClosestNode(target), out path)) //Generate path between two points on grid that are close to the transform position and the assigned target.
        {
            if (searchShortcut && path.Count>0)
                pathLeftToGo = ShortenPath(path);
            else
            {
                pathLeftToGo = new List<Vector2>(path);
                if (!snapToGrid) pathLeftToGo.Add(target);
            }

        }
    }


    /// <summary>
    /// Finds closest point on the grid
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    Vector2 GetClosestNode(Vector2 target) 
    {
        return new Vector2(Mathf.Round(target.x/gridSize)*gridSize, Mathf.Round(target.y / gridSize) * gridSize);
    }

    /// <summary>
    /// A distance approximation. 
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    float GetDistance(Vector2 A, Vector2 B) 
    {
        return (A - B).sqrMagnitude; //Uses square magnitude to lessen the CPU time.
    }

    /// <summary>
    /// Finds possible conenctions and the distances to those connections on the grid.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Dictionary<Vector2,float> GetNeighbourNodes(Vector2 pos) 
    {
        Dictionary<Vector2, float> neighbours = new Dictionary<Vector2, float>();
        for (int i=-1;i<2;i++)
        {
            for (int j=-1;j<2;j++)
            {
                if (i == 0 && j == 0) continue;

                Vector2 dir = new Vector2(i, j)*gridSize;
                if (!Physics2D.Linecast(pos,pos+dir, obstacles))
                {
                    neighbours.Add(GetClosestNode( pos + dir), dir.magnitude);
                }
            }

        }
        return neighbours;
    }

    
    List<Vector2> ShortenPath(List<Vector2> path)
    {
        List<Vector2> newPath = new List<Vector2>();
        
        for (int i=0;i<path.Count;i++)
        {
            newPath.Add(path[i]);
            for (int j=path.Count-1;j>i;j-- )
            {
                if (!Physics2D.Linecast(path[i],path[j], obstacles))
                {
                    
                    i = j;
                    break;
                }
            }
            newPath.Add(path[i]);
        }
        newPath.Add(path[path.Count - 1]);
        return newPath;
    }

}
