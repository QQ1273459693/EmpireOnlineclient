using UnityEngine;

namespace LeafUtilsRays
{

    public static class MeshUtils
    {
        /// <summary>
        /// [0] <- bottom left, [1] <- bottom right, [2]<- top right, [3] <- top left
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Vector2[] GetWorldCorners2D(MeshRenderer meshr)
        {
            Vector2[] corners = new Vector2[4];
            corners[0] = new Vector2(meshr.bounds.center.x - meshr.bounds.extents.x, meshr.bounds.center.y - meshr.bounds.extents.y);
            corners[1] = new Vector2(meshr.bounds.center.x + meshr.bounds.extents.x, meshr.bounds.center.y - meshr.bounds.extents.y);
            corners[2] = new Vector2(meshr.bounds.center.x + meshr.bounds.extents.x, meshr.bounds.center.y + meshr.bounds.extents.y);
            corners[3] = new Vector2(meshr.bounds.center.x - meshr.bounds.extents.x, meshr.bounds.center.y + meshr.bounds.extents.y);
            return corners;
        }

        public static Vector3[] GeVertsLocalSpace(Vector3[] vertsWorld, MeshFilter mesh)
        {
            Vector3[] vertsLocal = new Vector3[vertsWorld.Length];
            for (int i = 0; i < vertsLocal.Length; i++) vertsLocal[i] = mesh.transform.InverseTransformPoint(vertsWorld[i]);
            return vertsLocal;
        }

        public static Vector3[] GeVertsWorldSpace(MeshFilter mesh)
        {
            Vector3[] vertsWorld = new Vector3[mesh.sharedMesh.vertices.Length];
            for (int i = 0; i < vertsWorld.Length; i++) vertsWorld[i] = mesh.transform.TransformPoint(mesh.sharedMesh.vertices[i]);
            return vertsWorld;
        }

        public static Vector3[] GeVertsWorldSpace(Vector3[] verts, Transform mesh)
        {
            Vector3[] vertsWorld = new Vector3[verts.Length];
            for (int i = 0; i < vertsWorld.Length; i++) vertsWorld[i] = mesh.TransformPoint(verts[i]);
            return vertsWorld;
        }
    }

}