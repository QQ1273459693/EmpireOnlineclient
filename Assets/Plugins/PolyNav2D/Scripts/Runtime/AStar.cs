using System;
using System.Collections.Generic;
using UnityEngine;
using PathNode = PolyNav.PolyNavMap.PathNode;

namespace PolyNav
{

    ///<summary>Calculates paths using A*</summary>
    public static class AStar
    {

        ///<summary>Calculate path from startnode to endnode from within allnodes. Callbacks the resulting path</summary>
        public static void CalculatePath(PathNode startNode, PathNode endNode, List<PathNode> allNodes, Action<Vector2[]> callback) {
            var path = Internal_CalculatePath(startNode, endNode, allNodes);
            callback(path);
        }

        //...
        private static Vector2[] Internal_CalculatePath(PathNode startNode, PathNode endNode, List<PathNode> allNodes) {

            var openList = new Heap<PathNode>(allNodes.Count);
            var closedList = new HashSet<PathNode>();
            var success = false;

            openList.Add(startNode);

            while ( openList.Count > 0 ) {

                var currentNode = openList.RemoveFirst();
                if ( currentNode == endNode ) {
                    success = true;
                    break;
                }

                closedList.Add(currentNode);

                var linkIndeces = currentNode.links;
                for ( var i = 0; i < linkIndeces.Count; i++ ) {
                    var neighbour = allNodes[linkIndeces[i]];

                    if ( closedList.Contains(neighbour) ) {
                        continue;
                    }

                    var costToNeighbour = currentNode.gCost + ( currentNode.pos - neighbour.pos ).magnitude;
                    if ( costToNeighbour < neighbour.gCost || !openList.Contains(neighbour) ) {
                        neighbour.gCost = costToNeighbour;
                        neighbour.hCost = ( neighbour.pos - endNode.pos ).magnitude;
                        neighbour.parent = currentNode;

                        if ( !openList.Contains(neighbour) ) {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            if ( success ) { //Retrace path if one exists
                var path = new List<Vector2>();
                var currentNode = endNode;
                while ( currentNode != startNode ) {
                    path.Add(currentNode.pos);
                    currentNode = currentNode.parent;
                }
                path.Add(startNode.pos);
                path.Reverse();
                return path.ToArray();
            }

            return null;
        }
    }
}