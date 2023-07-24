using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PolyNav
{

    [DisallowMultipleComponent]
    [AddComponentMenu("Navigation/PolyNavAgent")]
    ///<summary>Place this on a game object to find it's path</summary>
    public class PolyNavAgent : MonoBehaviour
    {

        ///<summary>The target PolyNav2D map this agent is assigned to.</summary>
        [SerializeField]
        private PolyNavMap _map = null;

        [Header("Steering")]
        ///<summary>The max speed</summary>
		public float maxSpeed = 3.5f;
        ///<summary>The max steering force applied. Works like acceleration.</summary>
        public float maxForce = 10f;
        ///<summary>The distance to stop at from the goal</summary>
        public float stoppingDistance = 0.1f;
        ///<summary>The distance to start slowing down</summary>
        public float slowingDistance = 1;
        ///<summary>The lookahead distance for Slowing down and agent avoidance. Set to 0 to eliminate the slowdown but the avoidance too, as well as increase performance.</summary>
        public float lookAheadDistance = 1;

        [Header("Avoidance")]
        ///<summary>The avoidance radius of the agent. 0 for no avoidance	</summary>
		public float avoidRadius = 0;
        ///<summary>The max time in seconds where the agent is actively avoiding before considered "stuck".</summary>
        public float avoidanceConsiderStuckedTime = 3f;
        ///<summary>The max remaining path distance which will be considered reached, when the agent is "stuck".</summary>
        public float avoidanceConsiderReachedDistance = 1f;

        [Header("Rotation")]
        ///<summary>Rotate transform as well?</summary>
		public bool rotateTransform = false;
        ///<summary>Speed to rotate at moving direction.</summary>
        public float rotateSpeed = 350;

        [Header("Options")]
        ///<summary>Custom center offset from original transform position.</summary>
        public Vector2 centerOffset = Vector2.zero;
        ///<summary>Should the agent repath? Disable for performance.</summary>
        public bool repath = true;
        ///<summary>Should the agent be forced restricted within valid areas? Disable for performance.</summary>
        public bool restrict = false;
        ///<summary>Go to closer point if requested destination is invalid? Disable for performance.</summary>
        public bool closerPointOnInvalid = true;
        ///<summary>Will debug the path (gizmos). Disable for performance.</summary>
        public bool debugPath = true;


        ///<summary>Raised when a new destination is started after path found</summary>
        public event System.Action OnNavigationStarted;
        ///<summary>Raised when the destination is reached</summary>
        public event System.Action OnDestinationReached;
        ///<summary>Raised when the destination is or becomes invalid</summary>
        public event System.Action OnDestinationInvalid;
        ///<summary>Raised when a "corner" point has been reached while traversing the path</summary>
        public event System.Action<Vector2> OnNavigationPointReached;


        private event System.Action<bool> reachedCallback;

        ///----------------------------------------------------------------------------------------------

        private Vector2 currentVelocity = Vector2.zero;
        private int requests = 0;
        private List<Vector2> _activePath = new List<Vector2>();
        private Vector2 _primeGoal = Vector2.zero;

        private static List<PolyNavAgent> allAgents = new List<PolyNavAgent>();

        ///----------------------------------------------------------------------------------------------

        ///<summary>The position of the agent</summary>
        public Vector2 position {
            get { return transform.position + (Vector3)centerOffset; }
            set { transform.position = new Vector3(value.x, value.y, transform.position.z) - (Vector3)centerOffset; }
        }

        ///<summary>The current active path of the agent</summary>
        public List<Vector2> activePath {
            get { return _activePath; }
            set
            {
                _activePath = value;
                if ( _activePath.Count > 0 && _activePath[0] == position ) {
                    _activePath.RemoveAt(0);
                }
            }
        }

        ///<summary>The current goal of the agent</summary>
        public Vector2 primeGoal {
            get { return _primeGoal; }
            set { _primeGoal = value; }
        }

        ///<summary>Is a path pending?</summary>
        public bool pathPending {
            get { return requests > 0; }
        }

        ///<summary>The PolyNav map instance the agent is assigned to.</summary>
        public PolyNavMap map {
            get { return _map != null ? _map : PolyNavMap.current; }
            set { _map = value; }
        }

        ///<summary>Does the agent has a path?</summary>
        public bool hasPath {
            get { return activePath.Count > 0; }
        }

        ///<summary>The point that the agent is currenty going to. Returns the agent position if no active path</summary>
        public Vector2 nextPoint {
            get { return hasPath ? activePath[0] : position; }
        }

        ///<summary>The remaining distance of the active path. 0 if none</summary>
        public float remainingDistance {
            get
            {
                if ( !hasPath ) {
                    return 0;
                }

                float dist = Vector2.Distance(position, activePath[0]);
                for ( int i = 0; i < activePath.Count; i++ ) {
                    dist += Vector2.Distance(activePath[i], activePath[i == activePath.Count - 1 ? i : i + 1]);
                }

                return dist;
            }
        }

        ///<summary>The moving direction of the agent</summary>
        public Vector2 movingDirection {
            get { return hasPath ? currentVelocity.normalized : Vector2.zero; }
        }

        ///<summary>The current speed of the agent</summary>
        public float currentSpeed {
            get { return currentVelocity.magnitude; }
        }

        ///<summary>Is the agent currently actively avoiding another agent?</summary>
        public bool isAvoiding { get; private set; }

        ///<summary>The elapsed time in seconds in which the agent is actively avoiding another agent.</summary>
        public float avoidingElapsedTime { get; private set; }

        ///----------------------------------------------------------------------------------------------

        void OnEnable() { allAgents.Add(this); }
        void OnDisable() { allAgents.Remove(this); }
        void Awake() {
            primeGoal = position;
            if ( _map == null ) {
                _map = FindObjectsOfType<PolyNavMap>().FirstOrDefault(m => m.PointIsValid(position));
            }
        }

        ///<summary>Set the destination for the agent. As a result the agent starts moving</summary>
        public bool SetDestination(Vector2 goal) { return SetDestination(goal, null); }

        ///<summary>Set the destination for the agent. As a result the agent starts moving. Only the callback from the last SetDestination will be called upon arrival</summary>
        public bool SetDestination(Vector2 goal, Action<bool> callback) {

            if ( map == null ) {
                Debug.LogError("No PolyNavMap assigned and none exists in the scene!");
                return false;
            }

            //goal is almost the same as the last goal. Nothing happens for performace in case it's called frequently
            if ( ( goal - primeGoal ).magnitude < Mathf.Epsilon ) {
                return true;
            }

            reachedCallback = callback;
            primeGoal = goal;

            //goal is almost the same as agent position. We consider arrived immediately
            if ( ( goal - position ).magnitude < stoppingDistance ) {
                OnArrived();
                return true;
            }

            //check if goal is valid
            if ( !map.PointIsValid(goal) ) {
                if ( closerPointOnInvalid ) {
                    SetDestination(map.GetCloserEdgePoint(goal), callback);
                    return true;
                } else {
                    OnInvalid();
                    return false;
                }
            }

            //if a path is pending dont calculate new path
            //the prime goal will be repathed anyway
            if ( requests > 0 ) {
                return true;
            }

            //compute path
            requests++;
            map.FindPath(position, goal, SetPath);

            return true;
        }

        ///<summary>Clears the path and as a result the agent is stop moving</summary>
        public void Stop() {
            activePath.Clear();
            currentVelocity = Vector2.zero;
            requests = 0;
            primeGoal = position;
            avoidingElapsedTime = 0;
        }


        //the callback from map for when path is ready to use
        void SetPath(Vector2[] path) {

            //in case the agent stoped somehow, but a path was pending
            if ( requests == 0 ) {
                return;
            }

            requests--;

            if ( path == null || path.Length == 0 ) {
                OnInvalid();
                return;
            }

            activePath = path.ToList();
            if ( OnNavigationStarted != null ) {
                OnNavigationStarted();
            }
        }


        //main loop
        void LateUpdate() {

            if ( map == null ) {
                return;
            }

            //when there is no path just restrict
            if ( !hasPath ) {
                Restrict();
                return;
            }

            if ( maxSpeed <= 0 ) {
                return;
            }

            var targetVelocity = currentVelocity;
            // calculate velocities
            if ( remainingDistance < slowingDistance ) {
                targetVelocity += Arrive(nextPoint);
            } else { targetVelocity += Seek(nextPoint); }

            //move the agent
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, maxForce * Time.deltaTime);
            currentVelocity = Vector2.ClampMagnitude(currentVelocity, maxSpeed);

            //slow down if wall ahead and avoid other agents
            LookAhead();

            position += currentVelocity * Time.deltaTime;


            ///----------------------------------------------------------------------------------------------


            //check active avoidance elapsed time (= stuck)
            if ( isAvoiding && avoidingElapsedTime >= avoidanceConsiderStuckedTime ) {
                if ( remainingDistance > avoidanceConsiderReachedDistance ) {
                    OnInvalid();
                } else {
                    OnArrived();
                }
            }

            //restrict just after movement
            Restrict();

            //rotate if must
            if ( rotateTransform ) {
                float rot = -Mathf.Atan2(movingDirection.x, movingDirection.y) * 180 / Mathf.PI;
                float newZ = Mathf.MoveTowardsAngle(transform.localEulerAngles.z, rot, rotateSpeed * Time.deltaTime);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, newZ);
            }

            if ( repath ) {

                //repath if there is no LOS with the next point
                if ( map.CheckLOS(position, nextPoint) == false ) {
                    Repath();
                }

                //in case just after repath-ing there is no path
                if ( !hasPath ) {
                    OnArrived();
                    return;
                }
            }

            //Check and remove if we reached a point. proximity distance depends
            if ( hasPath ) {
                float proximity = ( activePath[activePath.Count - 1] == nextPoint ) ? stoppingDistance : 0.001f;
                if ( ( position - nextPoint ).magnitude <= proximity ) {

                    activePath.RemoveAt(0);

                    //if it was last point, means the path is complete and no longer have an active path.
                    if ( !hasPath ) {

                        OnArrived();
                        return;

                    } else {

                        if ( repath ) {
                            //repath after a point is reached
                            Repath();
                        }

                        if ( OnNavigationPointReached != null ) {
                            OnNavigationPointReached(position);
                        }
                    }
                }
            }

            //little trick. Check the next waypoint ahead of the current for LOS and if true consider the current reached.
            //helps for tight corners and when agent has big innertia
            if ( activePath.Count > 1 && map.CheckLOS(position, activePath[1]) ) {
                activePath.RemoveAt(0);
                if ( OnNavigationPointReached != null ) {
                    OnNavigationPointReached(position);
                }
            }
        }


        ///----------------------------------------------------------------------------------------------

        //seeking a target
        Vector2 Seek(Vector2 target) {

            var desiredVelocity = ( target - position ).normalized * maxSpeed;
            var steer = desiredVelocity - currentVelocity;
            return steer;
        }

        //slowing at target's arrival
        Vector2 Arrive(Vector2 target) {

            var desiredVelocity = ( target - position ).normalized * maxSpeed;
            desiredVelocity *= remainingDistance / slowingDistance;
            var steer = desiredVelocity - currentVelocity;
            return steer;
        }

        //slowing when there is an obstacle ahead.
        void LookAhead() {

            //if agent is outside dont LookAhead since that causes agent to constantely be slow.
            if ( lookAheadDistance <= 0 || !map.PointIsValid(position) ) {
                return;
            }

            var currentLookAheadDistance = Mathf.Lerp(0, lookAheadDistance, currentVelocity.magnitude / maxSpeed);
            var lookAheadPos = position + ( currentVelocity.normalized * currentLookAheadDistance );

            Debug.DrawLine(position, lookAheadPos, Color.blue);

            if ( !map.PointIsValid(lookAheadPos) ) {
                currentVelocity -= ( lookAheadPos - position );
            }

            //avoidance
            if ( avoidRadius > 0 ) {

                isAvoiding = false;
                for ( var i = 0; i < allAgents.Count; i++ ) {
                    var otherAgent = allAgents[i];
                    if ( otherAgent == this || otherAgent.avoidRadius <= 0 ) {
                        continue;
                    }

                    var mlt = otherAgent.avoidRadius + this.avoidRadius;
                    var dist = ( lookAheadPos - otherAgent.position ).magnitude;
                    var str = ( lookAheadPos - otherAgent.position ).normalized * mlt;
                    var steer = Vector3.Lerp((Vector3)str, Vector3.zero, dist / mlt);
                    if ( !isAvoiding ) { isAvoiding = steer.magnitude > 0; }
                    currentVelocity += ( (Vector2)steer ) * currentVelocity.magnitude;

                    Debug.DrawLine(otherAgent.position, otherAgent.position + str, new Color(1, 0, 0, 0.1f));
                }

                if ( isAvoiding ) {
                    avoidingElapsedTime += Time.deltaTime;
                } else {
                    avoidingElapsedTime = 0;
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        //stop the agent and callback + message
        void OnArrived() {

            Stop();

            if ( reachedCallback != null ) {
                reachedCallback(true);
            }

            if ( OnDestinationReached != null ) {
                OnDestinationReached();
            }
        }

        //stop the agent and callback + message
        void OnInvalid() {

            Stop();

            if ( reachedCallback != null ) {
                reachedCallback(false);
            }

            if ( OnDestinationInvalid != null ) {
                OnDestinationInvalid();
            }
        }

        //recalculate path to prime goal if there is no pending requests
        void Repath() {

            if ( requests > 0 ) {
                return;
            }

            requests++;
            map.FindPath(position, primeGoal, SetPath);
        }

        //keep agent within valid area
        void Restrict() {

            if ( !restrict ) {
                return;
            }

            if ( !map.PointIsValid(position) ) {
                position = map.GetCloserEdgePoint(position);
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        void OnDrawGizmos() {

            Gizmos.color = new Color(1, 1, 1, 0.1f);
            Gizmos.DrawWireSphere(position, avoidRadius);

            if ( !hasPath ) {
                return;
            }

            if ( debugPath ) {
                Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
                Gizmos.DrawLine(position, activePath[0]);
                for ( int i = 0; i < activePath.Count; i++ ) {
                    Gizmos.DrawLine(activePath[i], activePath[( i == activePath.Count - 1 ) ? i : i + 1]);
                }
            }
        }

#endif
        ///----------------------------------------------------------------------------------------------

    }
}