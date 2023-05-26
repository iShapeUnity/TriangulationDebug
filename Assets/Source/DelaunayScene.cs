using iShape.Geometry;
using iShape.Mesh2d;
using iShape.Triangulation.Shape.Delaunay;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Source {

    public class DelaunayScene : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private Mesh mesh;
        private int testIndex = 0;

        private void Awake() {
            Application.targetFrameRate = 60;
            this.meshFilter = gameObject.GetComponent<MeshFilter>();
            this.mesh = new Mesh();
            this.mesh.MarkDynamic();
            this.meshFilter.mesh = mesh;
            setTest(testIndex);
        }

        public void Next() {
            testIndex = (testIndex + 1) % TriangulationTests.Data.Length;
            setTest(testIndex);
        }

        private void setTest(int index) {
            var iGeom = IntGeom.DefGeom;

            var pShape = TriangulationTests.Data[index].ToPlainShape(iGeom, Allocator.Temp);
            var triangles = pShape.DelaunayTriangulate(Allocator.Temp);

            var points = iGeom.Float(pShape.points, Allocator.Temp);
            var vertices = new NativeArray<float3>(points.Length, Allocator.Temp);
            for (int i = 0; i < points.Length; ++i) {
                var p = points[i];
                vertices[i] = new float3(p.x, p.y, 0);
            }
            points.Dispose();
            
            var bodyMesh = new StaticPrimitiveMesh(vertices, triangles);
            
            vertices.Dispose();
            triangles.Dispose();
            
            var colorMesh = new NativeColorMesh(vertices.Length, Allocator.Temp);
            
            colorMesh.AddAndDispose(bodyMesh, Color.green);
            colorMesh.FillAndDispose(mesh);

            pShape.Dispose();
        }
    }

}
