using LeafUtilsRays;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace LightRays2D
{

    [Serializable]
    public class EditableSpriteQuad
    {
        public EditableSpriteQuad(GameObject parent)
        {
            this.id = parent.GetHashCode();
        }

        public bool Validate(GameObject parent) => this.id == parent.GetHashCode();

        public void SetAllCryoCallbacks(UnityAction a)
        {
            vertBottomLeft.onValueChanged = a; vertBottomRight.onValueChanged = a; vertTopLeft.onValueChanged = a; vertTopRight.onValueChanged = a;
        }

        public Cryo<Vector2> vertBottomLeft = new Cryo<Vector2>(new Vector2(-1, -1));
        public Cryo<Vector2> vertBottomRight = new Cryo<Vector2>(new Vector2(1, -1));
        public Cryo<Vector2> vertTopRight = new Cryo<Vector2>(new Vector2(1, 1));
        public Cryo<Vector2> vertTopLeft = new Cryo<Vector2>(new Vector2(-1, 1));
        public int id;

        public Vector3[] GetVerts()
        {
            Vector3[] v = { vertBottomLeft.value, vertBottomRight.value, vertTopRight.value, vertTopLeft.value };
            return v;
        }

        public Mesh GenerateSpriteQuad(Vector3[] verts = null, float size = 1)
        {
            if (verts == null)
            {
                vertBottomLeft = new Cryo<Vector2>(new Vector2(-1, -1) * size);
                vertBottomRight = new Cryo<Vector2>(new Vector2(1, -1) * size);
                vertTopRight = new Cryo<Vector2>(new Vector2(1, 1) * size);
                vertTopLeft = new Cryo<Vector2>(new Vector2(-1, 1) * size);
            }
            else
            {
                vertBottomLeft.value = verts[0];
                vertBottomRight.value = verts[1];
                vertTopRight.value = verts[2];
                vertTopLeft.value = verts[3];
            }

            Vector3[] vertices = new Vector3[4]
            {
            vertBottomLeft.value * size,
            vertBottomRight.value * size,
            vertTopRight.value * size,
            vertTopLeft.value * size,
            };

            int[] tris = new int[6]
            {
            0,2,1,
            0,3,2
            };

            Vector2[] uv = new Vector2[4]
            {
              new Vector2(0, 0),
              new Vector2(1, 0),
              new Vector2(1, 1),
              new Vector2(0, 1)
            };


            Vector2[] uv2 = new Vector2[4] {
            new Vector2(1,1f),
            new Vector2(1,1f),
            new Vector2(1,1f),
            new Vector2(1,1f),
        };

            float ax = vertices[2].x - vertices[0].x;
            float ay = vertices[2].y - vertices[0].y;
            float bx = vertices[3].x - vertices[1].x;
            float by = vertices[3].y - vertices[1].y;

            float cross = (ax * by) - (ay * bx);

            if (cross != 0)
            {
                float cy = vertices[0].y - vertices[1].y;
                float cx = vertices[0].x - vertices[1].x;

                float s = (ax * cy - ay * cx) / cross;

                if (s > 0 && s < 1)
                {
                    float t = (bx * cy - by * cx) / cross;

                    if (t > 0 && t < 1)
                    {
                        float q0 = 1 / (1 - t);
                        float q1 = 1 / (1 - s);
                        float q2 = 1 / t;
                        float q3 = 1 / s;
                        uv[0] = new Vector2(uv[0].x * q0, uv[0].y * q0);
                        uv[1] = new Vector2(uv[1].x * q1, uv[1].y * q1);
                        uv[2] = new Vector2(uv[2].x * q2, uv[2].y * q2);
                        uv[3] = new Vector2(uv[3].x * q3, uv[3].y * q3);
                        uv2[0] = new Vector2(q0, 0);
                        uv2[1] = new Vector2(q1, 0);
                        uv2[2] = new Vector2(q2, 0);
                        uv2[3] = new Vector2(q3, 0);
                    }
                }
            }

            Mesh m = new Mesh();
            m.vertices = vertices;
            m.triangles = tris;
            m.uv = uv;
            m.uv2 = uv2;
            m.RecalculateNormals();
            m.RecalculateBounds();

            return m;
        }

        public Mesh UpdateQuadUvs(Mesh m)
        {
            Vector3[] vertices = new Vector3[4]
            {
            m.vertices[0],
            m.vertices[1],
            m.vertices[2],
            m.vertices[3],
            };

            Vector2[] uv = new Vector2[4]
            {
              new Vector2(0, 0),
              new Vector2(1, 0),
              new Vector2(1, 1),
              new Vector2(0, 1)
             };


            Vector2[] uv2 = new Vector2[4] {
            new Vector2(1,1f),
            new Vector2(1,1f),
            new Vector2(1,1f),
            new Vector2(1,1f),
        };

            float ax = vertices[2].x - vertices[0].x;
            float ay = vertices[2].y - vertices[0].y;
            float bx = vertices[3].x - vertices[1].x;
            float by = vertices[3].y - vertices[1].y;

            float cross = (ax * by) - (ay * bx);

            if (cross != 0)
            {
                float cy = vertices[0].y - vertices[1].y;
                float cx = vertices[0].x - vertices[1].x;

                float s = (ax * cy - ay * cx) / cross;

                if (s > 0 && s < 1)
                {
                    float t = (bx * cy - by * cx) / cross;

                    if (t > 0 && t < 1)
                    {
                        float q0 = 1 / (1 - t);
                        float q1 = 1 / (1 - s);
                        float q2 = 1 / t;
                        float q3 = 1 / s;
                        uv[0] = new Vector2(uv[0].x * q0, uv[0].y * q0);
                        uv[1] = new Vector2(uv[1].x * q1, uv[1].y * q1);
                        uv[2] = new Vector2(uv[2].x * q2, uv[2].y * q2);
                        uv[3] = new Vector2(uv[3].x * q3, uv[3].y * q3);
                        uv2[0] = new Vector2(q0, 0);
                        uv2[1] = new Vector2(q1, 0);
                        uv2[2] = new Vector2(q2, 0);
                        uv2[3] = new Vector2(q3, 0);
                    }
                }
            }
            m.uv = uv;
            m.uv2 = uv2;
            return m;
        }
    }

}