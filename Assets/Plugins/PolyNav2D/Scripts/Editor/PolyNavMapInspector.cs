#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace PolyNav
{

    [CustomEditor(typeof(PolyNavMap))]
    public class PolyNavMapInspector : UnityEditor.Editor
    {

        private PolyNavMap map {
            get { return target as PolyNavMap; }
        }

        public override void OnInspectorGUI() {

            base.OnInspectorGUI();

            if ( GUI.changed ) {
                EditorApplication.delayCall += CheckChangeType;
            }

            if ( Application.isPlaying ) {
                EditorGUILayout.LabelField("Nodes Count", map.nodesCount.ToString());
            }
        }

        void CheckChangeType() {
            var collider = map.GetComponent<Collider2D>();
            var rb = map.GetComponent<Rigidbody2D>();
            if ( map.shapeType == PolyNavObstacle.ShapeType.Polygon && !( collider is PolygonCollider2D ) ) {
                if ( collider != null ) { UnityEditor.Undo.DestroyObjectImmediate(collider); }
                if ( rb != null ) { UnityEditor.Undo.DestroyObjectImmediate(map.GetComponent<Rigidbody2D>()); }
                var col = map.gameObject.AddComponent<PolygonCollider2D>();
                UnityEditor.Undo.RegisterCreatedObjectUndo(col, "Change Shape Type");
                map.invertMasterPolygon = true;
            }

            if ( map.shapeType == PolyNavObstacle.ShapeType.Box && !( collider is BoxCollider2D ) ) {
                if ( collider != null ) { UnityEditor.Undo.DestroyObjectImmediate(collider); }
                if ( rb != null ) { UnityEditor.Undo.DestroyObjectImmediate(map.GetComponent<Rigidbody2D>()); }
                var col = map.gameObject.AddComponent<BoxCollider2D>();
                UnityEditor.Undo.RegisterCreatedObjectUndo(col, "Change Shape Type");
                map.invertMasterPolygon = false;
            }

            if ( map.shapeType == PolyNavObstacle.ShapeType.Composite && !( collider is CompositeCollider2D ) ) {
                if ( collider != null ) { UnityEditor.Undo.DestroyObjectImmediate(collider); }
                var col = map.gameObject.AddComponent<CompositeCollider2D>();
                UnityEditor.Undo.RegisterCreatedObjectUndo(col, "Change Shape Type");
                rb = map.GetComponent<Rigidbody2D>();
                rb.simulated = false;
                map.invertMasterPolygon = true;
            }
        }
    }
}

#endif