using iShape.Geometry;
using iShape.Geometry.Container;
using iShape.Mesh2d;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Source {

    public class TriangulationJobHandler {

        private TriangulationJob job;
        private JobHandle jobHandle;
        
        public void Invoke(NativeArray<Vector2> hull, NativeArray<Vector2> hole, bool isDelaunay) {
            var iGeom = IntGeom.DefGeom;

            var iHull = iGeom.Int(hull.ToArray());
            var iHoles = new[] { iGeom.Int(hole.ToArray()) };

            var iShape = new IntShape(iHull, iHoles);
            var pShape = new PlainShape(iShape, Allocator.TempJob);

            int totalCount = pShape.points.Length * 3;

            var triangles = new NativeArray<int>(totalCount, Allocator.TempJob);
            var length = new NativeArray<int>(1, Allocator.TempJob);
            
            this.job = new TriangulationJob {
                plainShape = pShape,
                isDelaunay = isDelaunay,
                triangles = triangles,
                length = length
            };
            
            this.jobHandle = this.job.Schedule();
        }

        public NativeColorMesh Complete(Allocator allocator) {
            this.jobHandle.Complete();
            
            int n = this.job.length[0];
            var triangles = new NativeArray<int>(n, allocator);
            triangles.Slice(0, n).CopyFrom(job.triangles.Slice(0, n));
            
            var plainShape = job.plainShape;
            
            var points = IntGeom.DefGeom.Float(plainShape.points, allocator);
            var vertices = new NativeArray<float3>(points.Length, Allocator.Temp);
            for (int i = 0; i < points.Length; ++i) {
                var p = points[i];
                vertices[i] = new float3(p.x, p.y, 0);
            }
            points.Dispose();
            
            var bodyMesh = new StaticPrimitiveMesh(vertices, triangles);
            
            vertices.Dispose();
            triangles.Dispose();
            
            var mesh = new NativeColorMesh(vertices.Length, allocator);
            
            mesh.AddAndDispose(bodyMesh, Color.green);

            this.job.Dispose();

            return mesh;
        }

    }
}