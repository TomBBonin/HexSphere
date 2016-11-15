using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class HexSphereService : Service<HexSphereService>
{
    #region Icosahedron Consts
    private const float X = 0.525731112119133606f;
    private const float Z = 0.850650808352039932f;
    private const int NbIcosahedronFaces = 20;

    private static readonly Vector3[] IsocahedronVertices =
    {
        new Vector3( 0, Z, X), new Vector3( 0,  Z, -X), new Vector3( Z,  X,  0), new Vector3(X,  0,  Z),
        new Vector3(-X, 0, Z), new Vector3(-Z,  X,  0), new Vector3(-X,  0, -Z), new Vector3(X,  0, -Z),
        new Vector3(Z, -X, 0), new Vector3( 0, -Z,  X), new Vector3(-Z, -X,  0), new Vector3(0, -Z, -X)
    };

    private const float HalfTriangleX = 1f / 11f;
    private const float TriangleHeight = 1f / 3f;
    private static readonly Vector2[] IsocahedronUvs =
    {
        new Vector2(HalfTriangleX * 2f,  1f),                       // 0A
        new Vector2(HalfTriangleX     ,  TriangleHeight * 2f),      // 1A
        new Vector2(HalfTriangleX * 3f,  TriangleHeight * 2f),      // 2
        new Vector2(HalfTriangleX * 5f,  TriangleHeight * 2f),      // 3
        new Vector2(HalfTriangleX * 7f,  TriangleHeight * 2f),      // 4
        new Vector2(HalfTriangleX * 9f,  TriangleHeight * 2f),      // 5
        new Vector2(0f                ,  TriangleHeight),           // 6A
        new Vector2(HalfTriangleX * 2f,  TriangleHeight),           // 7
        new Vector2(HalfTriangleX * 4f,  TriangleHeight),           // 8
        new Vector2(HalfTriangleX * 6f,  TriangleHeight),           // 9
        new Vector2(HalfTriangleX * 8f,  TriangleHeight),           // 10
        new Vector2(HalfTriangleX     ,  0f),                       // 11A
        new Vector2(HalfTriangleX * 3f,  0f),                       // 11B (12)
        new Vector2(HalfTriangleX * 5f,  0f),                       // 11C (13)
        new Vector2(HalfTriangleX * 7f,  0f),                       // 11D (14)
        new Vector2(HalfTriangleX * 9f,  0f),                       // 11E (15)
        new Vector2(HalfTriangleX * 4f,  1f),                       // 0B  (16)
        new Vector2(HalfTriangleX * 6f,  1f),                       // 0C  (17)
        new Vector2(HalfTriangleX * 8f,  1f),                       // 0D  (18)
        new Vector2(HalfTriangleX * 10f, 1f),                       // 0E  (19)
        new Vector2(1f                 , TriangleHeight * 2f),      // 1B  (20)
        new Vector2(HalfTriangleX * 10f, TriangleHeight),           // 6B  (21)
    };

    private static readonly Vector2[][] IsocahedronTriangleUvs =
    {
        new [] { IsocahedronUvs[ 1], IsocahedronUvs[ 2], IsocahedronUvs[ 0] },
        new [] { IsocahedronUvs[ 2], IsocahedronUvs[ 3], IsocahedronUvs[16] },
        new [] { IsocahedronUvs[ 3], IsocahedronUvs[ 4], IsocahedronUvs[17] },
        new [] { IsocahedronUvs[ 4], IsocahedronUvs[ 5], IsocahedronUvs[18] },
        new [] { IsocahedronUvs[ 5], IsocahedronUvs[20], IsocahedronUvs[19] },
        new [] { IsocahedronUvs[ 6], IsocahedronUvs[ 7], IsocahedronUvs[ 1] },
        new [] { IsocahedronUvs[ 7], IsocahedronUvs[ 2], IsocahedronUvs[ 1] },
        new [] { IsocahedronUvs[ 7], IsocahedronUvs[ 8], IsocahedronUvs[ 2] },
        new [] { IsocahedronUvs[ 8], IsocahedronUvs[ 3], IsocahedronUvs[ 2] },
        new [] { IsocahedronUvs[ 8], IsocahedronUvs[ 9], IsocahedronUvs[ 3] },
        new [] { IsocahedronUvs[ 9], IsocahedronUvs[ 4], IsocahedronUvs[ 3] },
        new [] { IsocahedronUvs[ 9], IsocahedronUvs[10], IsocahedronUvs[ 4] },
        new [] { IsocahedronUvs[10], IsocahedronUvs[ 5], IsocahedronUvs[ 4] },
        new [] { IsocahedronUvs[10], IsocahedronUvs[21], IsocahedronUvs[ 5] },
        new [] { IsocahedronUvs[21], IsocahedronUvs[20], IsocahedronUvs[ 5] },
        new [] { IsocahedronUvs[11], IsocahedronUvs[ 7], IsocahedronUvs[ 6] },
        new [] { IsocahedronUvs[12], IsocahedronUvs[ 8], IsocahedronUvs[ 7] },
        new [] { IsocahedronUvs[13], IsocahedronUvs[ 9], IsocahedronUvs[ 8] },
        new [] { IsocahedronUvs[14], IsocahedronUvs[10], IsocahedronUvs[ 9] },
        new [] { IsocahedronUvs[15], IsocahedronUvs[21], IsocahedronUvs[10] }
    };

    private static readonly int[][] IsocahedronTriangles =
    {
        new []{1,  2, 0}, new []{ 2,  3, 0}, new []{ 3,  4, 0}, new []{ 4,  5, 0}, new []{ 5, 1,  0},
        new []{6,  7, 1}, new []{ 7,  2, 1}, new []{ 7,  8, 2}, new []{ 8,  3, 2}, new []{ 8, 9,  3},
        new []{9,  4, 3}, new []{ 9, 10, 4}, new []{10,  5, 4}, new []{10,  6, 5}, new []{ 6, 1,  5},
        new []{11, 7, 6}, new []{11,  8, 7}, new []{11,  9, 8}, new []{11, 10, 9}, new []{11, 6, 10}
    };
    #endregion

    public int Resolution = 0;
    public Material Material;
    public Transform World;

    private int _nbVertices;
    private List<Vector3> _spherePoints;
    private List<Vector2> _sphereUvs;
    private List<int> _sphereTris;

    private const int MaxResolutionWithSingleMesh = 5;
    private const int MaxResolutionBeforeMeshSubdivision = 7;

    protected override void InitService()
    {
        var container = new GameObject { name = "GeodesicSphere_" + Resolution + "_Subdivisions" };
        container.transform.parent = World;

        var isSingleMesh = Resolution <= MaxResolutionWithSingleMesh;

        _nbVertices = NbIcosahedronFaces * 3 * (int)Mathf.Pow(4, Resolution); // 20 triangles on an icosahedron, 3 vertices per triangle, each triangle when subdivided yields 4 triangles
        var verticesPerFace = _nbVertices / NbIcosahedronFaces;
        var meshSubdivisionDepth = (int)Mathf.Pow(4, Mathf.Max(0, Resolution - MaxResolutionBeforeMeshSubdivision));
        var nbMeshSubdivisions = isSingleMesh ? 1 : NbIcosahedronFaces * meshSubdivisionDepth;
        var verticesPerMesh = isSingleMesh ? _nbVertices : verticesPerFace / meshSubdivisionDepth;

        Debug.Log("Total Verts [" + _nbVertices + "]");
        Debug.Log("Verts per Face [" + verticesPerFace + "]");
        Debug.Log("Verts per Mesh [" + verticesPerMesh + "]");
        Debug.Log("Total Mesh Subdivisions [" + nbMeshSubdivisions + "]");

        _spherePoints = new List<Vector3>(_nbVertices);
        _sphereUvs = new List<Vector2>(_nbVertices);
        _sphereTris = new List<int>(verticesPerMesh);

        //FunctionTimer.START_FUNCTION_TIMER("Subdivision_Total");
        for (var i = 0; i < NbIcosahedronFaces; i++)
        {
            Subdivide(IsocahedronVertices[IsocahedronTriangles[i][0]],
                IsocahedronVertices[IsocahedronTriangles[i][1]],
                IsocahedronVertices[IsocahedronTriangles[i][2]],
                IsocahedronTriangleUvs[i][0],
                IsocahedronTriangleUvs[i][1],
                IsocahedronTriangleUvs[i][2],
                Resolution, i == 0 || isSingleMesh);
        }
        //FunctionTimer.STOP_FUNCTION_TIMER("Subdivision_Total");

        for (var i = 0; i < nbMeshSubdivisions; i++)
        {
            var startIdx = isSingleMesh ? 0 : i * verticesPerMesh;
            var mesh = new Mesh
            {
                name = isSingleMesh ? "GeodesicSphere_Mesh" : "GeodesicSphere_Face_" + i + "_Mesh",
                vertices = _spherePoints.GetRange(startIdx, verticesPerMesh).ToArray(),
                uv = _sphereUvs.GetRange(startIdx, verticesPerMesh).ToArray(),
                triangles = _sphereTris.ToArray()
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            CalculateMeshTangents(mesh);
            mesh.Optimize();

            var obj = new GameObject { name = isSingleMesh ? "GeodesicSphere" : "GeodesicSphere_Face" + i };
            var objRenderer = obj.AddComponent<MeshRenderer>();
            var objFilter = obj.AddComponent<MeshFilter>();
            objFilter.mesh = mesh;
            objRenderer.sharedMaterial = Material;
            obj.transform.parent = container.transform;
        }
    }

    private void Subdivide(Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3, int depth, bool addTris = false)
    {
        if (depth == 0)
        {
            _spherePoints.Add(v1);
            _spherePoints.Add(v2);
            _spherePoints.Add(v3);

            _sphereUvs.Add(uv1);
            _sphereUvs.Add(uv2);
            _sphereUvs.Add(uv3);

            if (addTris)
            {
                _sphereTris.Add(_spherePoints.Count - 1);
                _sphereTris.Add(_spherePoints.Count - 2);
                _sphereTris.Add(_spherePoints.Count - 3);
            }
            return;
        }
        var v12 = (v1 + v2).normalized;
        var v23 = (v2 + v3).normalized;
        var v31 = (v3 + v1).normalized;

        var uv12 = (uv1 + uv2) / 2f;
        var uv23 = (uv2 + uv3) / 2f;
        var uv31 = (uv3 + uv1) / 2f;

        var addTrisDepth = addTris && depth <= 7;
        Subdivide(v1, v12, v31, uv1, uv12, uv31, depth - 1, addTris);
        Subdivide(v2, v23, v12, uv2, uv23, uv12, depth - 1, addTrisDepth);
        Subdivide(v3, v31, v23, uv3, uv31, uv23, depth - 1, addTrisDepth);
        Subdivide(v12, v23, v31, uv12, uv23, uv31, depth - 1, addTrisDepth);
    }

    private static void CalculateMeshTangents(Mesh mesh)
    {
        // Recalculate mesh tangents
        // I found this on the internet (Unity forums?), I don't take credit for it.
        // speed up math by copying the mesh arrays
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;
        var uv = mesh.uv;
        var normals = mesh.normals;

        //variable definitions
        var triangleCount = triangles.Length;
        var vertexCount = vertices.Length;

        var tan1 = new Vector3[vertexCount];
        var tan2 = new Vector3[vertexCount];
        var tangents = new Vector4[vertexCount];

        for (long a = 0; a < triangleCount; a += 3)
        {
            long i1 = triangles[a + 0];
            long i2 = triangles[a + 1];
            long i3 = triangles[a + 2];

            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];

            var w1 = uv[i1];
            var w2 = uv[i2];
            var w3 = uv[i3];

            var x1 = v2.x - v1.x;
            var x2 = v3.x - v1.x;
            var y1 = v2.y - v1.y;
            var y2 = v3.y - v1.y;
            var z1 = v2.z - v1.z;
            var z2 = v3.z - v1.z;

            var s1 = w2.x - w1.x;
            var s2 = w3.x - w1.x;
            var t1 = w2.y - w1.y;
            var t2 = w3.y - w1.y;

            var r = 1.0f / (s1 * t2 - s2 * t1);

            var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }

        for (long a = 0; a < vertexCount; ++a)
        {
            var n = normals[a];
            var t = tan1[a];

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
}