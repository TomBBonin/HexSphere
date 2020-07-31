using System;
using UnityEngine;

namespace Thomas
{
    public class DebugService : Service<DebugService>
    {
        protected override void InitService() { }
        
        private Color[] Colors = {Color.blue, Color.cyan, Color.red, Color.magenta, Color.yellow, Color.green, Color.gray, Color.black, Color.white};

        private enum DebugView
        {
            None,
            HexTris,
            FaceRelations,
            TriCellRelations
        }
        private DebugView _debugView;
        private int _currentResolution;

        protected void Update()
        {
            if (Input.GetKeyUp(KeyCode.F1))
                AssignDebugView(DebugView.HexTris);
            if (Input.GetKeyUp(KeyCode.F2))
                AssignDebugView(DebugView.FaceRelations);
            if (Input.GetKeyUp(KeyCode.F3))
                AssignDebugView(DebugView.TriCellRelations);


            if (Input.GetKeyUp(KeyCode.P))
                _currentResolution = Mathf.Max(_currentResolution + 1, 1 /*HexSphereService.I.Resolution*/);
            if (Input.GetKeyUp(KeyCode.M))
                _currentResolution = Mathf.Max(_currentResolution - 1, 0);
        }

        private void AssignDebugView(DebugView newDebugView)
        {
            _debugView = _debugView == newDebugView ? DebugView.None : newDebugView;
        }
        
        protected void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            
            switch (_debugView)
            {
                case DebugView.None: return;
                case DebugView.HexTris: DrawHexTris(null); break;
                case DebugView.FaceRelations: DrawTriRelations(null); break;
                case DebugView.TriCellRelations: DrawTriCellRelations(null); break;
            }
        }
        
        private void DrawHexTris(HexSphereService.Triangle tri)
        {
            if (tri == null)
            { 
                DrawHexTris(HexSphereService.I._triangles[0]);
                DrawHexTris(HexSphereService.I._triangles[6]);
                return;
            }
            
            if (tri.Resolution > _currentResolution)
            {
                foreach (var child in tri.Children.Values)
                    DrawHexTris(child);
                return;
            }

            var i = 0;
            foreach (var child in tri.Children.Values)
            {
                Gizmos.color = Colors[i];
                Vector3 ctr;
                if (_currentResolution == 1)
                {
                    var pt = child.GeoTri.PointIdx;
                    ctr = (HexSphereService._spherePoints[pt] + HexSphereService._spherePoints[pt + 1] + HexSphereService._spherePoints[pt + 2]) / 3f;
                }
                else
                    ctr = HexSphereService._spherePoints[child.Children[HexSphereService.SubTriangles.HexTopRight].GeoTri.PointIdx];
                Gizmos.DrawWireSphere(ctr, 0.025f);
                i++;
            }
        }

        private void DrawTriRelations(HexSphereService.Triangle tri)
        {
            if (tri == null)
            {
                foreach (var face in HexSphereService.I._triangles)
                    DrawTriRelations(face);
                return;
            }

            if (tri.Resolution > _currentResolution)
            {
                foreach (var child in tri.Children.Values)
                    DrawTriRelations(child);
                return;
            }

            // HexTopRight.GeoTri.PointIdx is the center of the face
            Vector3 ctr;
            Vector3 leftCtr;
            Vector3 rightCtr;
            Vector3 topBotCtr;

            if (_currentResolution > 0)
            {
                ctr = HexSphereService._spherePoints[tri.Children[HexSphereService.SubTriangles.HexTopRight].GeoTri.PointIdx];
                leftCtr = HexSphereService._spherePoints[tri.Left.Children[HexSphereService.SubTriangles.HexTopRight].GeoTri.PointIdx];
                rightCtr = HexSphereService._spherePoints[tri.Right.Children[HexSphereService.SubTriangles.HexTopRight].GeoTri.PointIdx];
                topBotCtr = HexSphereService._spherePoints[tri.TopBot.Children[HexSphereService.SubTriangles.HexTopRight].GeoTri.PointIdx];
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(ctr, 0.025f);
            }
            else
            {
                ctr = GetTriCtr(tri.GeoTri.PointIdx);
                leftCtr = GetTriCtr(tri.Left.GeoTri.PointIdx);
                rightCtr = GetTriCtr(tri.Right.GeoTri.PointIdx);
                topBotCtr = GetTriCtr(tri.TopBot.GeoTri.PointIdx);
            }
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(ctr, rightCtr);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ctr, leftCtr);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ctr, topBotCtr);
        }

        private Vector3 GetTriCtr(int idx)
        {
            return (HexSphereService._spherePoints[idx] + HexSphereService._spherePoints[idx + 1] + HexSphereService._spherePoints[idx + 2]) / 3f;
        }

        private void DrawTriCellRelations(HexSphereService.Triangle tri)
        {
            if (tri == null)
            {
                foreach (var baseTri in HexSphereService.I._triangles)
                    DrawTriCellRelations(baseTri);
                return;
            }
            
            if (tri.Resolution > 1)
            {
                foreach (var child in tri.Children.Values)
                    DrawTriCellRelations(child);
                return;
            }

            Vector3 hexctr = Vector3.zero;
            if (tri.Cells.ContainsKey(HexSphereService.TriangleCells.Hex))
            {
                hexctr = tri.Cells[HexSphereService.TriangleCells.Hex].Normal;
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(hexctr, 0.025f);
            }

            if (tri.Cells.ContainsKey(HexSphereService.TriangleCells.Right))
            {
                var rightCtr = tri.Cells[HexSphereService.TriangleCells.Right].Normal;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(hexctr, rightCtr);
            }

            if (tri.Cells.ContainsKey(HexSphereService.TriangleCells.Left))
            {
                var leftCtr = tri.Cells[HexSphereService.TriangleCells.Left].Normal;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(hexctr, leftCtr);
            }

            if (tri.Cells.ContainsKey(HexSphereService.TriangleCells.TopBot))
            {
                var topBotCtr = tri.Cells[HexSphereService.TriangleCells.TopBot].Normal;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(hexctr, topBotCtr);
            }
        }
    }
}