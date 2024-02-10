using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Kamgam.UGUIBlurredBackground
{
    public partial class BlurredBackgroundImage
    {
        [Header("Mesh")]
        public bool UseCustomMesh = false;

        [Range(1, 32)]
        public int CustomMeshDivisions = 16;

        /// <summary>
        /// Callback function when a UI element needs to generate vertices. Fills the vertex buffer data.
        /// </summary>
        /// <param name="vh">VertexHelper utility.</param>
        /// <remarks>
        /// Used by Text, UI.Image, and RawImage for example to generate vertices specific to their use case.
        /// </remarks>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // Don't draw anything if blur strength is 0.
            if (Iterations == 0 || Strength == 0)
            {
                vh.Clear();
                canvasRenderer.Clear();
                return;
            }

            // Create mesh
            if (UseCustomMesh)
            {
                drawCustomMesh(vh);
            }
            else
            {
                base.OnPopulateMesh(vh);
            }
            updateUVs(vh);

            // Use blur texture
            var texture = BlurManager.Instance.GetBlurredTexture(GetRenderMode());
            if (texture != null)
                canvasRenderer.SetTexture(texture);
        }

        protected void drawCustomMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect r = GetPixelAdjustedRect();

            int divisionX = CustomMeshDivisions;
            int divisionY = CustomMeshDivisions;

            float divWidth = r.width / divisionX;
            float divHeight = r.height / divisionY;
            
            var color32 = color;

            for (int x = 0; x < divisionX; x++)
            {
                for (int y = 0; y < divisionY; y++)
                {
                    var posMin = new Vector3(r.xMin + x * divWidth, r.yMin + y * divHeight);
                    var posMax = new Vector3(r.xMin + (x+1) * divWidth, r.yMin + (y+1) * divHeight);
                    var uvMin = new Vector2(posMin.x / r.width, posMin.y / r.height);
                    var uvMax = new Vector2(posMax.x / r.width, posMax.y / r.height);
                    AddQuad(
                        vh,
                        posMin, posMax, color32,
                        uvMin, uvMax
                    );
                }
            }
        }

        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        protected static bool _typeInfoCached = false;
        protected static FieldInfo _uvField;
        protected static FieldInfo _vertexField;

        protected static void cacheTypeInfo()
        {
            if (_typeInfoCached)
                return;

            _typeInfoCached = true;

            var type = typeof(VertexHelper);
            _uvField = type.GetField("m_Uv0S", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            _vertexField = type.GetField("m_Positions", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Accesses vertices and UVs directly and updates UVs to
        /// be front projected (in screen space) based on vertex positions.
        /// </summary>
        /// <param name="vh"></param>
        protected void updateUVs(VertexHelper vh)
        {
            cacheTypeInfo();

            if (_uvField == null || _vertexField == null)
                return;

            List<Vector3> vertices = _vertexField.GetValue(vh) as List<Vector3>;
            List<Vector4> uvs = _uvField.GetValue(vh) as List<Vector4>;

            if (vertices == null || uvs == null || vertices.Count != uvs.Count)
                return;

            var gameCam = RenderUtils.GetGameViewCamera(this);

            // Abort if no or wrong camera.
            if (gameCam == null)
                return;

            for (int i = 0; i < uvs.Count; i++)
            {
                uvs[i] = calculateFrontProjectedUV(vertices[i], gameCam);
            }
        }

        // NOTICE:
        // For world or camera space canvases this ONLY fixes the vertices. The interpolation
        // on the pixels within every triangle is done by the default UI shader. That shader
        // logically assumes that it needs to do perspective distortion on the texture.
        // However,
        // the texture (blurred screen) we are using was already rendered in perspective and thus
        // it will be distorted. The solution to this would be to make a separate "front-projected"
        // shader for the image material or patch the texture (rotate) to match the world object
        // leading to one texture per object (expensive). For now, I opted not to do any of those.
        protected Vector4 calculateFrontProjectedUV(Vector3 vertexPos, Camera cam)
        {
            var worldPos = getWorldPosition(vertexPos);

            // Convert from world to UV (our texture covers the whole screen).
            Vector4 uv = worldPos;

            // If we are NOT in a "screen space overlay" canvas then we have to convert
            // to screen space first.
            if (GetRenderMode() != RenderMode.ScreenSpaceOverlay)
            {
                uv = cam.WorldToScreenPoint(uv);
            }

            // The divide by the screen size to get a nice 0 to 1 range.
            uv.x /= cam.pixelWidth;
            uv.y /= cam.pixelHeight;

            // Return the new UV (now "front projected").
            return uv;
        }

        /// <summary>
        /// Returns the screen position for the given local coordinate.
        /// </summary>
        /// <returns></returns>
        protected Vector4 getWorldPosition(Vector3 localPos)
        {
            Matrix4x4 matrix4x = transform.localToWorldMatrix;
            return matrix4x.MultiplyPoint(localPos);
        }
    }
}