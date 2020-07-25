using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Thomas
{
    public class HexSphereService : Service<HexSphereService>
    {
        #region Icosahedron Consts

        private const float X = 0.525731112119133606f;
        private const float Z = 0.850650808352039932f;
        private const int NbIcosahedronFaces = 20;

        private static readonly Vector3[] IcosahedronVertices =
        {
            new Vector3(0, Z, X), new Vector3(0, Z, -X), new Vector3(Z, X, 0), new Vector3(X, 0, Z),
            new Vector3(-X, 0, Z), new Vector3(-Z, X, 0), new Vector3(-X, 0, -Z), new Vector3(X, 0, -Z),
            new Vector3(Z, -X, 0), new Vector3(0, -Z, X), new Vector3(-Z, -X, 0), new Vector3(0, -Z, -X)
        };

        private const float HalfTriangleX = 1f / 11f;
        private const float TriangleHeight = 1f / 3f;

        private static readonly Vector2[] IcosahedronUvs =
        {
            new Vector2(HalfTriangleX * 2f, 1f), // 0A
            new Vector2(HalfTriangleX, TriangleHeight * 2f), // 1A
            new Vector2(HalfTriangleX * 3f, TriangleHeight * 2f), // 2
            new Vector2(HalfTriangleX * 5f, TriangleHeight * 2f), // 3
            new Vector2(HalfTriangleX * 7f, TriangleHeight * 2f), // 4
            new Vector2(HalfTriangleX * 9f, TriangleHeight * 2f), // 5
            new Vector2(0f, TriangleHeight), // 6A
            new Vector2(HalfTriangleX * 2f, TriangleHeight), // 7
            new Vector2(HalfTriangleX * 4f, TriangleHeight), // 8
            new Vector2(HalfTriangleX * 6f, TriangleHeight), // 9
            new Vector2(HalfTriangleX * 8f, TriangleHeight), // 10
            new Vector2(HalfTriangleX, 0f), // 11A
            new Vector2(HalfTriangleX * 3f, 0f), // 11B (12)
            new Vector2(HalfTriangleX * 5f, 0f), // 11C (13)
            new Vector2(HalfTriangleX * 7f, 0f), // 11D (14)
            new Vector2(HalfTriangleX * 9f, 0f), // 11E (15)
            new Vector2(HalfTriangleX * 4f, 1f), // 0B  (16)
            new Vector2(HalfTriangleX * 6f, 1f), // 0C  (17)
            new Vector2(HalfTriangleX * 8f, 1f), // 0D  (18)
            new Vector2(HalfTriangleX * 10f, 1f), // 0E  (19)
            new Vector2(1f, TriangleHeight * 2f), // 1B  (20)
            new Vector2(HalfTriangleX * 10f, TriangleHeight), // 6B  (21)
        };

        private static readonly Vector2[][] IcosahedronTriangleUvs =
        {
            new[] {IcosahedronUvs[1], IcosahedronUvs[2], IcosahedronUvs[0]},
            new[] {IcosahedronUvs[2], IcosahedronUvs[3], IcosahedronUvs[16]},
            new[] {IcosahedronUvs[3], IcosahedronUvs[4], IcosahedronUvs[17]},
            new[] {IcosahedronUvs[4], IcosahedronUvs[5], IcosahedronUvs[18]},
            new[] {IcosahedronUvs[5], IcosahedronUvs[20], IcosahedronUvs[19]},
            new[] {IcosahedronUvs[6], IcosahedronUvs[7], IcosahedronUvs[1]},
            new[] {IcosahedronUvs[7], IcosahedronUvs[2], IcosahedronUvs[1]},
            new[] {IcosahedronUvs[7], IcosahedronUvs[8], IcosahedronUvs[2]},
            new[] {IcosahedronUvs[8], IcosahedronUvs[3], IcosahedronUvs[2]},
            new[] {IcosahedronUvs[8], IcosahedronUvs[9], IcosahedronUvs[3]},
            new[] {IcosahedronUvs[9], IcosahedronUvs[4], IcosahedronUvs[3]},
            new[] {IcosahedronUvs[9], IcosahedronUvs[10], IcosahedronUvs[4]},
            new[] {IcosahedronUvs[10], IcosahedronUvs[5], IcosahedronUvs[4]},
            new[] {IcosahedronUvs[10], IcosahedronUvs[21], IcosahedronUvs[5]},
            new[] {IcosahedronUvs[21], IcosahedronUvs[20], IcosahedronUvs[5]},
            new[] {IcosahedronUvs[11], IcosahedronUvs[7], IcosahedronUvs[6]},
            new[] {IcosahedronUvs[12], IcosahedronUvs[8], IcosahedronUvs[7]},
            new[] {IcosahedronUvs[13], IcosahedronUvs[9], IcosahedronUvs[8]},
            new[] {IcosahedronUvs[14], IcosahedronUvs[10], IcosahedronUvs[9]},
            new[] {IcosahedronUvs[15], IcosahedronUvs[21], IcosahedronUvs[10]}
        };

        private static readonly int[][] IcosahedronTriangles =
        {
            new[] {1, 2, 0}, new[] {2, 3, 0}, new[] {3, 4, 0}, new[] {4, 5, 0}, new[] {5, 1, 0},
            new[] {6, 7, 1}, new[] {7, 2, 1}, new[] {7, 8, 2}, new[] {8, 3, 2}, new[] {8, 9, 3},
            new[] {9, 4, 3}, new[] {9, 10, 4}, new[] {10, 5, 4}, new[] {10, 6, 5}, new[] {6, 1, 5},
            new[] {11, 7, 6}, new[] {11, 8, 7}, new[] {11, 9, 8}, new[] {11, 10, 9}, new[] {11, 6, 10}
        };

        private static readonly int[][] IcosahedronTrianglesNeighbours =
        {
            new[] {1, 4, 6},
            new[] {2, 0, 8},
            new[] {3, 1, 10},
            new[] {4, 2, 12},
            new[] {0, 3, 14},

            new[] {6, 14, 15},
            new[] {7, 5, 0},
            new[] {8, 6, 16},
            new[] {9, 7, 1},
            new[] {10, 8, 17},
            new[] {11, 9, 2},
            new[] {12, 10, 18},
            new[] {13, 11, 3},
            new[] {14, 12, 19},
            new[] {5, 13, 4},

            new[] {16, 19, 5},
            new[] {17, 15, 7},
            new[] {18, 16, 9},
            new[] {19, 17, 11},
            new[] {15, 18, 13}
        };

        #endregion

        public bool CreateEveryVersion;
        public int Resolution = 0;
        public int SmoothingAngle = 60;
        public Material Material;
        public Transform World;

        private int _nbVertices;
        private static List<Vector3> _spherePoints;
        private List<Vector2> _sphereUvs;
        private List<int> _sphereTris;

        private const int MaxResolutionWithSingleMesh = 5;
        private const int MaxResolutionBeforeMeshSubdivision = 7;

        protected override void InitService()
        {
            if (CreateEveryVersion)
            {
                for (var i = 0; i < Resolution; i++)
                    CreateGeodesicSphere(i);
            }
            else
                CreateGeodesicSphere(Resolution);
        }

        private void CreateGeodesicSphere(int resolution)
        {
            var container = new GameObject {name = "GeodesicSphere_" + resolution + "_Subdivisions"};
            container.transform.parent = World;
            var pos = Vector3.zero;
            if (CreateEveryVersion)
            {
                pos.x = 2.5f * (resolution % 4);
                pos.y = 2.5f - (2 * resolution / 8f) * 2.5f;
            }

            container.transform.position = pos;

            var isSingleMesh = resolution <= MaxResolutionWithSingleMesh;

            _nbVertices = NbIcosahedronFaces * 3 * (int) Mathf.Pow(9, resolution); // 20 triangles on an icosahedron, 3 vertices per triangle, each triangle when subdivided yields 4 triangles
            var verticesPerFace = _nbVertices / NbIcosahedronFaces;
            var meshSubdivisionDepth = (int) Mathf.Pow(4, Mathf.Max(0, resolution - MaxResolutionBeforeMeshSubdivision));
            var nbMeshSubdivisions = isSingleMesh ? 1 : NbIcosahedronFaces * meshSubdivisionDepth;
            var verticesPerMesh = isSingleMesh ? _nbVertices : verticesPerFace / meshSubdivisionDepth;

            Debug.Log("Total Verts [" + _nbVertices + "]");
            Debug.Log("Verts per Face [" + verticesPerFace + "]");
            Debug.Log("Verts per Mesh [" + verticesPerMesh + "]");
            Debug.Log("Total Mesh Subdivisions [" + nbMeshSubdivisions + "]");

            _spherePoints = new List<Vector3>(_nbVertices);
            _sphereUvs = new List<Vector2>(_nbVertices);
            _sphereTris = new List<int>(verticesPerMesh);
            _centers = new List<Vector3>();
            _col = new List<Color>();
            _rng = new List<float>();
            _cells = new Dictionary<Vector2Int, Cell>();
            _tests = new List<Vector3>();
            InitTriangles(resolution);

            FunctionTimer.START_FUNCTION_TIMER("Subdivision_Total_" + resolution);
            for (var i = 0; i < NbIcosahedronFaces; i++)
            {
                SubdivideGeo(IcosahedronVertices[IcosahedronTriangles[i][0]],
                    IcosahedronVertices[IcosahedronTriangles[i][1]],
                    IcosahedronVertices[IcosahedronTriangles[i][2]],
                    IcosahedronTriangleUvs[i][0],
                    IcosahedronTriangleUvs[i][1],
                    IcosahedronTriangleUvs[i][2],
                    _triangles[i],
                    resolution);
                //i == 0 || isSingleMesh);
            }
            
            InitCells();

            FunctionTimer.STOP_FUNCTION_TIMER("Subdivision_Total_" + resolution);

            // Go through each cell, and adjust the center point to create a flat surface
            foreach (var cell in _cells.Values)
                cell.Flatten(_spherePoints);

            foreach (var ctr in _centers)
            {
                _col.Add(new Color().Random());
                _rng.Add(Random.Range(0.025f, 0.075f));
            }

            FunctionTimer.START_FUNCTION_TIMER("MeshCreation_Total_" + resolution);
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
                mesh.RecalculateNormals(SmoothingAngle);
                mesh.RecalculateBounds();
                CalculateMeshTangents(mesh);

                var obj = new GameObject {name = isSingleMesh ? "GeodesicSphere" : "GeodesicSphere_Face" + i};
                var objRenderer = obj.AddComponent<MeshRenderer>();
                var objFilter = obj.AddComponent<MeshFilter>();
                objFilter.mesh = mesh;
                objRenderer.sharedMaterial = Material;
                obj.transform.SetParent(container.transform, false);
            }

            FunctionTimer.STOP_FUNCTION_TIMER("MeshCreation_Total_" + resolution);
        }

        private void InitTriangles(int resolution)
        {
            _triangles = new List<Triangle>
            {
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 1
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 2
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 3
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 4
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 5
                
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 6
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 7
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 8
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 9
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 10
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 11
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 12
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 13
                new Triangle(TriangleOrientation.Up, SubTriangles.None, resolution), // 14
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 15
                
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 16
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 17
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 18
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution), // 19
                new Triangle(TriangleOrientation.Down, SubTriangles.None, resolution) // 20
            };

            for (var i = 0; i < IcosahedronTrianglesNeighbours.Length; i++)
            {
                _triangles[i].AddNeighbours(
                    _triangles[IcosahedronTrianglesNeighbours[i][0]],
                    _triangles[IcosahedronTrianglesNeighbours[i][1]],
                    _triangles[IcosahedronTrianglesNeighbours[i][2]]);
            }

            // strictly greater than 0, the last tris have no children
            for (var i = resolution; i > 0; i--)
            {
                foreach (var triangle in _triangles)
                    triangle.BuildChildrenNeighbors(i);
            }
        }

        private void InitCells()
        {
            var originTri = _triangles[0];
            while (originTri.Resolution > 0)
                originTri = originTri.Children[SubTriangles.TriTopBot];
            
            BuildPartialCell(originTri, Vector2Int.zero, TriangleCells.TopBot);
            // Flood fill cells from origin
            var topTri = _triangles[0];
            while (topTri.Resolution > 1)
                topTri = topTri.Children[SubTriangles.TriTopBot];
            
            topTri.BuildHexCell(_cells, new Vector2Int(0, 1));
            
            //foreach (var triangle in _triangles)
            //    triangle.BuildHexCells(_cells);
        }

        private static Cell BuildPartialCell(Triangle tri, Vector2Int pos, TriangleCells cellDir)
        {
            var cell = tri.CreateCell(_cells, pos, cellDir);
            var newTri = tri;
            do
            {
                cell.AddTris(newTri.GeoTri);
                newTri = newTri.Right;
            } while (newTri != tri);

            return cell;
        }

        private int GetCtrVert(SubTriangles tri, bool isUp)
        {
            switch (tri)
            {
                case SubTriangles.None: return 0;
                case SubTriangles.HexBot: return 2;
                case SubTriangles.HexBotLeft: return 2;
                case SubTriangles.HexBotRight: return 0;
                case SubTriangles.HexTop: return 1;
                case SubTriangles.HexTopLeft: return 1;
                case SubTriangles.HexTopRight: return 0;
                case SubTriangles.TriLeft: return 0;
                case SubTriangles.TriRight: return isUp ? 1 : 2;
                case SubTriangles.TriTopBot: return isUp ? 2 : 1;
            }

            return 0;
        }
        
/*
        public int GetFlattenVert(SubTriangles tri, bool isUp)
        {
            switch (tri)
            {
                case SubTriangles.None: break;
                case SubTriangles.HexBot: break;
                case SubTriangles.HexBotLeft: break;
                case SubTriangles.HexBotRight: break;
                case SubTriangles.HexTop: break;
                case SubTriangles.HexTopLeft: break;
                case SubTriangles.HexTopRight: break;
                case SubTriangles.TriLeft: return isUp ? 0 : 1;
                case SubTriangles.TriRight: break;
                case SubTriangles.TriTopBot: break;
            }
        }
  */      
        private void SubdivideGeo(Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3, Triangle parent, int depth)
        {
            if (depth == 0)
            {
                parent.AddGeoTri(_spherePoints.Count, _sphereUvs.Count, _sphereTris.Count, GetCtrVert(parent.Id, parent.IsUp));
                
                // Add info to parent to be able to go through all 
                _spherePoints.Add(v1);
                _spherePoints.Add(v2);
                _spherePoints.Add(v3);

                _sphereUvs.Add(uv1);
                _sphereUvs.Add(uv2);
                _sphereUvs.Add(uv3);

                _sphereTris.Add(_spherePoints.Count - 1);
                _sphereTris.Add(_spherePoints.Count - 2);
                _sphereTris.Add(_spherePoints.Count - 3);

                return;
            }

            var vctr = (v1 + v2 + v3).normalized;
            var v12a = (v1 + (v2 - v1) / 3f).normalized;
            var v12b = (v1 + (v2 - v1) / 3f * 2f).normalized;
            var v23a = (v2 + (v3 - v2) / 3f).normalized;
            var v23b = (v2 + (v3 - v2) / 3f * 2f).normalized;
            var v13a = (v1 + (v3 - v1) / 3f).normalized;
            var v13b = (v1 + (v3 - v1) / 3f * 2f).normalized;

            var uvctr = (uv1 + uv2 + uv3) / 3f;
            var uv12a = (uv1 + (uv2 - uv1) / 3f);
            var uv12b = (uv1 + (uv2 - uv1) / 3f * 2f);
            var uv23a = (uv2 + (uv3 - uv2) / 3f);
            var uv23b = (uv2 + (uv3 - uv2) / 3f * 2f);
            var uv13a = (uv1 + (uv3 - uv1) / 3f);
            var uv13b = (uv1 + (uv3 - uv1) / 3f * 2f);

            var nextDepth = depth - 1;
            SubdivideGeo(v13b, v23b, v3, uv13b, uv23b, uv3, parent.Children[SubTriangles.TriTopBot], nextDepth); // tri top
            SubdivideGeo(v1, v12a, v13a, uv1, uv12a, uv13a, parent.Children[SubTriangles.TriLeft], nextDepth); // tri bot left
            SubdivideGeo(v12b, v2, v23a, uv12b, uv2, uv23a, parent.Children[SubTriangles.TriRight], nextDepth); // tri bot right
            
            SubdivideGeo(v12a, v12b, vctr, uv12a, uv12b, uvctr, parent.Children[SubTriangles.HexBot], nextDepth); // hex bot
            SubdivideGeo(v13a, v12a, vctr, uv13a, uv12a, uvctr, parent.Children[SubTriangles.HexBotLeft], nextDepth); // hex bot left
            SubdivideGeo(vctr, v12b, v23a, uvctr, uv12b, uv23a, parent.Children[SubTriangles.HexBotRight], nextDepth); // hex bot right
            SubdivideGeo(vctr, v23a, v23b, uvctr, uv23a, uv23b, parent.Children[SubTriangles.HexTopRight], nextDepth); // hex top right
            SubdivideGeo(v13a, vctr, v13b, uv13a, uvctr, uv13b, parent.Children[SubTriangles.HexTopLeft], nextDepth); // hex top left
            SubdivideGeo(v13b, vctr, v23b, uv13b, uvctr, uv23b, parent.Children[SubTriangles.HexTop], nextDepth); // hex top
        }

        private List<Triangle> _triangles;
        public enum TriangleOrientation { Up, Down }
        public enum SubTriangles
        {
            None,
            HexBot,
            HexBotLeft,
            HexBotRight,
            HexTop,
            HexTopLeft,
            HexTopRight,
            TriLeft,
            TriRight,
            TriTopBot
        }

        public class GeoTriangle
        {
            public readonly int PointIdx;
            public readonly int UVIdx;
            public readonly int TriIdx;
            public readonly int CtrVert; // +0, +1, or +2

            public GeoTriangle(int pt, int uv, int tri, int ctrVert)
            {
                PointIdx = pt;
                UVIdx = uv;
                TriIdx = tri;
                CtrVert = ctrVert;
            }
        }
        
        public enum TriangleCells { Hex, Left, Right, TopBot }
        
        private class Triangle
        {
            // Triangles face either up or down
            // Their Neighbors are always in Right, Left, Top Bottom order
            // 

            public int Resolution;
            public TriangleOrientation Orientation;
            public Dictionary<SubTriangles, Triangle> Children;
            public Triangle Right { get; private set; }
            public Triangle Left { get; private set;}
            public Triangle TopBot { get; private set;}
            public bool IsUp { get { return Orientation == TriangleOrientation.Up; } }
            public SubTriangles Id;

            public GeoTriangle GeoTri { get; private set; }
            public Dictionary<TriangleCells, Cell> Cells;

            public Triangle(TriangleOrientation orientation, SubTriangles id, int resolution)
            {
                Resolution = resolution;
                Orientation = orientation;
                Children = new Dictionary<SubTriangles, Triangle>();
                Cells = new Dictionary<TriangleCells, Cell>();
                Id = id;

                if (resolution == 0)
                    return;

                var nextResolution = resolution - 1;
                Children.Add(SubTriangles.TriTopBot, new Triangle(orientation, SubTriangles.TriTopBot, nextResolution));
                Children.Add(SubTriangles.TriLeft, new Triangle(orientation, SubTriangles.TriLeft, nextResolution));
                Children.Add(SubTriangles.TriRight, new Triangle(orientation, SubTriangles.TriRight, nextResolution));
                Children.Add(SubTriangles.HexBot, new Triangle(TriangleOrientation.Up, SubTriangles.HexBot, nextResolution));
                Children.Add(SubTriangles.HexBotLeft, new Triangle(TriangleOrientation.Down, SubTriangles.HexBotLeft, nextResolution));
                Children.Add(SubTriangles.HexBotRight, new Triangle(TriangleOrientation.Down, SubTriangles.HexBotRight, nextResolution));
                Children.Add(SubTriangles.HexTop, new Triangle(TriangleOrientation.Down, SubTriangles.HexTop, nextResolution));
                Children.Add(SubTriangles.HexTopLeft, new Triangle(TriangleOrientation.Up, SubTriangles.HexTopLeft, nextResolution));
                Children.Add(SubTriangles.HexTopRight, new Triangle(TriangleOrientation.Up, SubTriangles.HexTopRight, nextResolution));
            }

            public void BuildChildrenNeighbors(int resolution)
            {
                if (Resolution != resolution)
                {
                    foreach (var child in Children.Values)
                        child.BuildChildrenNeighbors(resolution);
                    return;
                }

                var hexbot = Children[SubTriangles.HexBot];
                var hexbotright = Children[SubTriangles.HexBotRight];
                var hexbotleft = Children[SubTriangles.HexBotLeft];
                var hextop = Children[SubTriangles.HexTop];
                var hextopright = Children[SubTriangles.HexTopRight];
                var hextopleft = Children[SubTriangles.HexTopLeft];
                var tritopbot = Children[SubTriangles.TriTopBot];
                var triright = Children[SubTriangles.TriRight];
                var trileft = Children[SubTriangles.TriLeft];
                
                tritopbot.AddNeighbours(
                    IsUp != Right.IsUp ? Right.Children[SubTriangles.TriLeft] : Right.Children[SubTriangles.TriTopBot],
                    IsUp != Left.IsUp ? Left.Children[SubTriangles.TriRight] : Left.Children[SubTriangles.TriTopBot],
                    IsUp ? hextop : hexbot);
                
                trileft.AddNeighbours(
                    IsUp ? hexbotleft : hextopleft,
                    IsUp != Left.IsUp ? Left.Children[SubTriangles.TriTopBot] : Left.Children[SubTriangles.TriRight],
                    TopBot.Children[SubTriangles.TriLeft]);

                triright.AddNeighbours(
                    IsUp != Right.IsUp ? Right.Children[SubTriangles.TriTopBot] : Right.Children[SubTriangles.TriLeft],
                    IsUp ? hexbotright : hextopright,
                    TopBot.Children[SubTriangles.TriRight]);
                
                hexbot.AddNeighbours(
                    hexbotright,
                    hexbotleft,
                    IsUp ? TopBot.Children[SubTriangles.HexTop] : tritopbot);
                
                hexbotleft.AddNeighbours(
                    hexbot,
                    IsUp ? 
                        trileft :
                        Left.IsUp ? 
                            Left.Children[SubTriangles.HexTopRight] :
                            Left.Children[SubTriangles.HexBotRight],
                    hextopleft);

                hexbotright.AddNeighbours(
                    IsUp ? 
                        triright : 
                        Right.IsUp ? 
                            Right.Children[SubTriangles.HexTopLeft] :
                            Right.Children[SubTriangles.HexBotLeft],
                    hexbot,
                    hextopright);

                hextop.AddNeighbours(
                    hextopright,
                    hextopleft,
                    IsUp ? tritopbot : TopBot.Children[SubTriangles.HexBot]);

                hextopleft.AddNeighbours(
                    hextop,
                    IsUp ?
                        Left.IsUp ? 
                            Right.Children[SubTriangles.HexTopRight] :
                            Right.Children[SubTriangles.HexBotRight] : 
                        trileft,
                    hexbotleft);

                hextopright.AddNeighbours(
                    IsUp ? 
                        Right.IsUp ? 
                            Right.Children[SubTriangles.HexTopLeft] :
                            Right.Children[SubTriangles.HexBotLeft] : 
                        triright,
                    hextop,
                    hexbotright);
            }

            public void AddNeighbours(Triangle right, Triangle left, Triangle topbot)
            {
                Right = right;
                Left = left;
                TopBot = topbot;
            }

            public void AddGeoTri(int pointIdx, int uvIdx, int triIdx, int ctrVert)
            {
                GeoTri = new GeoTriangle(pointIdx, uvIdx, triIdx, ctrVert);
            }
            
            public void BuildHexCell(Dictionary<Vector2Int, Cell> allCells, Vector2Int pos, bool stop = false)
            {
                if (!Cells.ContainsKey(TriangleCells.Hex))
                {
                    CreateCell(allCells, pos, TriangleCells.Hex,
                        Children[SubTriangles.HexBot].GeoTri,
                        Children[SubTriangles.HexBotLeft].GeoTri,
                        Children[SubTriangles.HexBotRight].GeoTri,
                        Children[SubTriangles.HexTop].GeoTri,
                        Children[SubTriangles.HexTopLeft].GeoTri,
                        Children[SubTriangles.HexTopRight].GeoTri);
                }

                if (stop)
                    return;
                
                if(!Right.Cells.ContainsKey(TriangleCells.Hex))
                    Right.BuildHexCell(allCells, new Vector2Int(pos.x + 1, pos.y));
                else if (Right.Cells[TriangleCells.Hex].X == 0)
                {
                    /*
                    if (!Right.Cells.ContainsKey(TriangleCells.Right))
                    {
                        List<GeoTriangle> geoTris = new List<GeoTriangle>();
                        geoTris.Add(Right.Children[SubTriangles.TriRight].GeoTri);
                        geoTris.Add(Right.Right.Children[SubTriangles.TriLeft].GeoTri);
                        geoTris.Add(Right.Right.TopBot.Children[SubTriangles.TriLeft].GeoTri);
                        geoTris.Add(Right.Right.TopBot.Left.Children[SubTriangles.TriTopBot].GeoTri);
                        geoTris.Add(Right.TopBot.Right.Children[SubTriangles.TriTopBot].GeoTri);
                        var last =  Right.TopBot.Children[SubTriangles.TriRight].GeoTri;
                        if(!geoTris.Contains(last)) 
                            geoTris.Add(last);

                        foreach (var tri in geoTris)
                            _tests.Add(_spherePoints[tri.CtrVert > 1 ? tri.PointIdx : tri.PointIdx + tri.CtrVert + 1]);
                        
                        var cell = Right.CreateCell(_cells, pos, TriangleCells.Right, geoTris.ToArray());
                        Right.Right.AddCell(cell, TriangleCells.Left);
                        Right.Right.TopBot.AddCell(cell, TriangleCells.Left);
                        Right.Right.TopBot.Left.AddCell(cell, TriangleCells.TopBot);
                        Right.TopBot.Right.AddCell(cell, TriangleCells.TopBot);
                        if(geoTris.Count > 5)
                            Right.TopBot.AddCell(cell, TriangleCells.Right);

                    }
                    */
                    if(!Right.TopBot.Cells.ContainsKey(TriangleCells.Hex))
                        Right.TopBot.BuildHexCell(allCells, new Vector2Int(Right.Cells[TriangleCells.Hex].X, pos.y + 1));
                    
                    else if (!TopBot.Cells.ContainsKey(TriangleCells.Hex))
                       TopBot.BuildHexCell(allCells, new Vector2Int(pos.x, pos.y + 1));
                }
                else if (!TopBot.Cells.ContainsKey(TriangleCells.Hex))
                    TopBot.BuildHexCell(allCells, new Vector2Int(pos.x, pos.y + 1));
            }

            public void BuildPartialCells(Dictionary<Vector2Int, Cell> cells)
            {
                CreateCell(cells, new Vector2Int(0, 0), TriangleCells.TopBot,
                    Children[SubTriangles.TriTopBot].GeoTri
                );
                CreateCell(cells, new Vector2Int(0, 0), TriangleCells.Left,
                    Children[SubTriangles.TriLeft].GeoTri
                );
                CreateCell(cells, new Vector2Int(0, 0),TriangleCells.Right,
                    Children[SubTriangles.TriRight].GeoTri
                );
            }
            
            public Cell CreateCell(Dictionary<Vector2Int, Cell> cells, Vector2Int pos, TriangleCells cellDir, params GeoTriangle[] tris)
            {
                Cell cell;
                if(cells.ContainsKey(pos))
                    cell = cells[pos];
                else
                {
                    cell = new Cell(pos);
                    cells.Add(cell.XY, cell);
                }

                cell.AddTris(tris);
                Cells.Add(cellDir, cell);
                return cell;
            }

            public void AddCell(Cell cell, TriangleCells cellDir)
            {
                Cells.Add(cellDir, cell);
            }
        }

        private static Dictionary<Vector2Int, Cell> _cells;

        private static bool once;
        private static bool onceB;
        
        private class Cell
        {
            public List<GeoTriangle> Tris;
            public Vector3 Normal;
            public Vector2Int XY;
            public int X { get { return XY.x; } }
            public int Y { get { return XY.y; } }

            public Cell(Vector2Int xy)
            {
                XY = xy;
                Tris = new List<GeoTriangle>(6);
            }

            public void AddTris(params GeoTriangle[] tris)
            {
                foreach (var tri in tris)
                    Tris.Add(tri);
            }

            public void Flatten(List<Vector3> pts)
            {
                var ctr = Tris.Count != 6 ? GetPolygonCtr(pts) : GetHexCtr(pts);
                foreach (var tri in Tris)
                    pts[tri.PointIdx + tri.CtrVert] = ctr; // flatten center vert to calculated ctr
                _centers.Add(ctr);
            }
            
            private Vector3 GetHexCtr(List<Vector3> pts)
            {
                var ctr = Vector3.zero;
                foreach (var tri in Tris)
                    ctr += pts[tri.CtrVert > 1 ? tri.PointIdx : tri.PointIdx + tri.CtrVert + 1];
                return ctr /= Tris.Count;
            }

            private Vector3 GetPolygonCtr(List<Vector3> pts)
            {
                var verts = new List<Vector3>();
                foreach (var tri in Tris)
                    verts.Add(pts[tri.CtrVert > 1 ? tri.PointIdx : tri.PointIdx + tri.CtrVert + 1]);
                return GetCentroid(verts.ToArray());
            }
        }
        
         private static Vector3 GetCentroid(Vector3[] vertices)
         {
             float sx = 0f, sy = 0f, sz = 0f, sL = 0f;
             Action<Vector3, Vector3> add = (prev, next) =>
             {
                 var l = Mathf.Pow(
                     Mathf.Pow(next.x - prev.x, 2) + 
                     Mathf.Pow(next.y - prev.y, 2) + 
                     Mathf.Pow(next.z - prev.z, 2), 0.5f);
                 sx += (prev.x + next.x) / 2f * l;
                 sy += (prev.y + next.y) / 2f * l;
                 sz += (prev.z + next.z) / 2f * l;
                 sL += l;
             };

             add(vertices[vertices.Length - 1], vertices[0]);
             for (var i = 1; i < vertices.Length; i++)
                 add(vertices[i - 1], vertices[i]);
             return new Vector3(sx, sy, sz) / sL;
        }
         private static List<Vector3> _tests;
         private static List<Vector3> _centers;
        private List<Color> _col;
        private static int testctr;

        private List<float> _rng;
        private Color[] cols = {Color.blue, Color.cyan, Color.red, Color.magenta, Color.yellow, Color.green};
        protected void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            Gizmos.color = Color.blue;
            if (_centers != null)
            {
                for (var i = 0; i < _centers.Count; i++)
                {
                    var ctr = _centers[i];
                    //Gizmos.color = _col[i];
                    Gizmos.DrawWireSphere(ctr, 0.025f); // _rng[i]
                }
            }

            for (var i = 0; i < _tests.Count; i++)
            {
                Gizmos.color = cols[i];
                Gizmos.DrawWireSphere(_tests[i], 0.025f);
            }

            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(_spherePoints[testctr], 0.05f); // _rng[i]
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_spherePoints[0], 0.025f); // _rng[i]

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

    public static class Extensions
    {
        public static Color Random(this Color col)
        {
            return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }
    }
}