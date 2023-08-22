using System;
using iShape.Geometry;
using iShape.Mesh2d;
using iShape.Triangulation.Shape.Delaunay;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source {

    public class TesselationScene : MonoBehaviour {

        [Range(min: 1f, max: 25)]
        [SerializeField]
        private float maxEdge = 1;
        
        [Range(min: 0.5f, max: 10)]
        [SerializeField]
        private float maxArea = 1;
        
        private MeshFilter meshFilter;
        private Mesh mesh;
        private int testIndex = 0;

        private float prevMaxEdge = 1;
        private float prevMaxArea = 1;
        
        private void Awake() {
            Application.targetFrameRate = 60;
            this.meshFilter = gameObject.GetComponent<MeshFilter>();
            this.mesh = new Mesh();
            this.mesh.MarkDynamic();
            this.meshFilter.mesh = mesh;
            prevMaxEdge = maxEdge;
            prevMaxArea = maxArea;
            
            setTest(testIndex);
        }

        private void Update() {
            if (Math.Abs(prevMaxEdge - maxEdge) > 0.01f || Math.Abs(prevMaxArea - maxArea) > 0.01f) {
                prevMaxEdge = maxEdge;
                prevMaxArea = maxArea;
                setTest(testIndex);
            }
        }

        public void Next() {
            testIndex = (testIndex + 1) % TriangulationTests.Data.Length;
            setTest(testIndex);
        }

        private void setTest(int index) {
            var iGeom = IntGeom.DefGeom;

            var pShape = TriangulationTests.Data[index].ToPlainShape(iGeom, Allocator.Temp);
            
            var extraPoints = new NativeArray<IntVector>(0, Allocator.Temp);
            var delaunay = pShape.Delaunay(iGeom.Int(maxEdge), extraPoints, Allocator.Temp);
            delaunay.Tessellate(iGeom, maxArea);
            
            extraPoints.Dispose();
            
            var triangles = delaunay.Indices(Allocator.Temp);
            var vertices = delaunay.Vertices(Allocator.Temp, iGeom, 0);
            
            delaunay.Dispose();

            // set each triangle as a separate mesh

            var subVertices = new NativeArray<float3>(3, Allocator.Temp);
            var subIndices = new NativeArray<int>(new [] {0, 1, 2 }, Allocator.Temp);

            var colorMesh = new NativeColorMesh(triangles.Length, Allocator.Temp);
            
            for (int i = 0; i < triangles.Length; i += 3) {

                for (int j = 0; j < 3; j += 1) {
                    var v = vertices[triangles[i + j]];
                    subVertices[j] = new float3(v.x, v.y, v.z);
                }
            
                var subMesh = new StaticPrimitiveMesh(subVertices, subIndices, Allocator.Temp);
                var color = new Color(Random.value, Random.value, Random.value);
                
                colorMesh.AddAndDispose(subMesh, color);
            }

            subIndices.Dispose();
            subVertices.Dispose();
            
            vertices.Dispose();
            triangles.Dispose();
            colorMesh.FillAndDispose(mesh);
            pShape.Dispose();
        }
    }

}