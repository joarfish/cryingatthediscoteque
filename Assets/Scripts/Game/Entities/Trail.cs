using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    namespace Entities
    {
        public class Trail : MonoBehaviour
        {
            public float thickness = 0.5f;
            public float max_distance = 1.0f;
            public float min_distance = 0.0f;

            List<Vector3> path = new List<Vector3>();
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            Mesh mesh;

            float uv_x = 0;

            public float Length
            {
                get;
                private set;
            } = 0.0f;

            private float lastSegmentLength = 0.0f;

            void Awake()
            {
                Material rainbowMat = new Material(Shader.Find("Unlit/Rainbowtrail"));
                gameObject.AddComponent<MeshRenderer>().material = rainbowMat;

                mesh = new Mesh { name = "TrailMesh" };
                gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            }


            public void AddPathPoint(Vector3 point)
            {
                if (path.Count < 2 || Vector3.Distance(point, path[path.Count - 2]) > min_distance)
                {
                    lastSegmentLength = 0.0f;
                    path.Add(point);
                    updatePathLength();

                    if (path.Count > 1)
                    {
                        UpdateMeshOnAddPoint();
                    }
                    else
                    {
                        CreateMesh();
                    }
                }
            }

            public void UpdateLastPathPoint(Vector3 point)
            {
                if (path.Count > 1)
                {
                    if (Vector3.Distance(point, path[path.Count - 2]) > max_distance)
                    {
                        AddPathPoint(point);
                    }
                    else
                    {
                        path[path.Count - 1] = point;
                        UpdateVerticesOnLastPoint();
                        updatePathLength();
                    }
                }
            }

            private void updatePathLength()
            {
                if (path.Count > 1)
                {
                    int pathPointCount = path.Count;

                    Length -= lastSegmentLength;
                    lastSegmentLength = Vector3.Distance(path[pathPointCount - 1], path[pathPointCount - 2]);
                    Length += lastSegmentLength;
                }

            }

            private void CreateMesh()
            {
                Vector3 dir = getCurrentDirection();

                uvs.Add(new Vector2(uv_x, 0));
                uvs.Add(new Vector2(uv_x, 1));

                Vector3 lastPoint = path[path.Count - 1];

                vertices.Add(new Vector3(lastPoint.x + dir.z * thickness, lastPoint.y, lastPoint.z + dir.x * -1 * thickness));
                vertices.Add(new Vector3(lastPoint.x + dir.z * -1 * thickness, lastPoint.y, lastPoint.z + dir.x * thickness));

                mesh.SetVertices(vertices);
                mesh.SetUVs(0, uvs);
                mesh.triangles = indices.ToArray();
            }

            private void RecreateMesh()
            {
                vertices.RemoveRange(0, vertices.Count);
                uvs.RemoveRange(0, uvs.Count);
                indices.RemoveRange(0, indices.Count);

                Vector3 dir = new Vector3();

                for (int p = 0, i = 0; p < path.Count - 1; p++, i += 2)
                {
                    if (p == 0)
                    {
                        dir = path[p + 1] - path[p];
                    }
                    else
                    {
                        dir = path[p + 1] - path[p - 1];
                    }

                    dir.Normalize();
                    vertices.Add(new Vector3(path[p].x + dir.z * thickness, path[p].y, path[p].z + dir.x * -1 * thickness));
                    vertices.Add(new Vector3(path[p].x + dir.z * -1 * thickness, path[p].y, path[p].z + dir.x * thickness));

                    uvs.Add(new Vector2(uv_x, 0));
                    uvs.Add(new Vector2(uv_x, 1));
                    uv_x = uv_x == 0 ? 1 : 0;

                    addQuadIndices(i);
                }

                uvs.Add(new Vector2(uv_x, 0));
                uvs.Add(new Vector2(uv_x, 1));

                Vector3 lastPoint = path[path.Count - 1];

                vertices.Add(new Vector3(path[path.Count - 1].x + dir.z * thickness, lastPoint.y, path[path.Count - 1].z + dir.x * -1 * thickness));
                vertices.Add(new Vector3(path[path.Count - 1].x + dir.z * -1 * thickness, lastPoint.y, path[path.Count - 1].z + dir.x * thickness));

                updateMesh();
            }

            private void UpdateMeshOnAddPoint()
            {
                int lastIndex = path.Count - 1;
                Vector3 newPoint = path[lastIndex];
                bool singleQuad = lastIndex == 1;
                Vector3 dir = getCurrentDirection();

                if (!singleQuad && Vector3.Dot(newPoint, path[lastIndex - 2]) < 0)
                {
                    dir = dir * -1;
                }

                int indexBase = vertices.Count - 2;

                if (!singleQuad)
                {
                    // Update vertices to account for new direction 
                    Vector3 oldPoint = path[path.Count - 2];
                    vertices[indexBase] = new Vector3(oldPoint.x + dir.z * thickness, oldPoint.y, oldPoint.z + dir.x * -1 * thickness);
                    vertices[indexBase + 1] = new Vector3(oldPoint.x + dir.z * -1 * thickness, oldPoint.y, oldPoint.z + dir.x * thickness);
                }

                // Add new vertex
                vertices.Add(new Vector3(newPoint.x + dir.z * thickness, newPoint.y, newPoint.z + dir.x * -1 * thickness));
                vertices.Add(new Vector3(newPoint.x + dir.z * -1 * thickness, newPoint.y, newPoint.z + dir.x * thickness));

                uvs.Add(new Vector2(uv_x, 0));
                uvs.Add(new Vector2(uv_x, 1));

                addQuadIndices(indexBase);
                updateMesh();

            }

            void UpdateVerticesOnLastPoint()
            {
                int lastIndex = path.Count - 1;
                Vector3 lastPoint = path[lastIndex];

                int indexBase = vertices.Count - 1;
                bool singleQuad = lastIndex == 1;

                Vector3 dir = getCurrentDirection();

                if (!singleQuad && Vector3.Dot(path[lastIndex - 1], path[lastIndex - 2]) < 0)
                {
                    dir = dir * -1;
                }

                if (!singleQuad)
                {
                    vertices[indexBase - 3] = new Vector3(path[path.Count - 2].x + dir.z * thickness, path[path.Count - 2].y, path[path.Count - 2].z + dir.x * -1 * thickness);
                    vertices[indexBase - 2] = new Vector3(path[path.Count - 2].x + dir.z * -1 * thickness, path[path.Count - 2].y, path[path.Count - 2].z + dir.x * thickness);
                }

                vertices[indexBase - 1] = new Vector3(lastPoint.x + dir.z * thickness, lastPoint.y, lastPoint.z + dir.x * -1 * thickness);
                vertices[indexBase] = new Vector3(lastPoint.x + dir.z * -1 * thickness, lastPoint.y, lastPoint.z + dir.x * thickness);

                updateMesh();
            }


            private void OnValidate()
            {
                if (mesh && path.Count != 0)
                {
                    RecreateMesh();
                }
            }

            private void OnDrawGizmosSelected()
            {

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }

                for (int i = 0; i < vertices.Count - 3; i += 2)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(vertices[i + 1], vertices[i + 3]);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(vertices[i], vertices[i + 2]);
                }

            }

            private Vector3 getCurrentDirection()
            {
                Vector3 dir;
                int lastIndex = path.Count - 1;

                if (path.Count < 2)
                {
                    dir = -Vector3.back;
                }
                else if (path.Count == 2)
                {
                    dir = path[lastIndex] - path[lastIndex - 1];
                }
                else
                {
                    dir = path[lastIndex] - path[lastIndex - 2];
                }

                dir.Normalize();

                return dir;
            }

            private void addQuadIndices(int indexBase)
            {
                indices.Add(indexBase + 3);
                indices.Add(indexBase + 2);
                indices.Add(indexBase);
                indices.Add(indexBase + 1);
                indices.Add(indexBase + 3);
                indices.Add(indexBase);
            }

            private void updateMesh()
            {
                mesh.Clear();
                mesh.SetVertices(vertices);
                mesh.SetUVs(0, uvs);
                mesh.triangles = indices.ToArray();
            }
        }

    }
}