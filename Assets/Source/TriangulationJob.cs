using iShape.Geometry.Container;
using iShape.Triangulation.Shape;
using iShape.Triangulation.Shape.Delaunay;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Source {
    
    [BurstCompile]
    public struct TriangulationJob: IJob {

        [ReadOnly]
        internal PlainShape plainShape;
        
        [ReadOnly]
        internal bool isDelaunay;

        [WriteOnly]
        internal NativeArray<int> triangles;
        
        [WriteOnly]
        internal NativeArray<int> length;

        public void Execute() {
            NativeArray<int> result;

            if (isDelaunay) {
                result = plainShape.DelaunayTriangulate(Allocator.Temp);    
            } else {
                result = plainShape.Triangulate(Allocator.Temp);
            }

            int n = result.Length;
            length[0] = n;

            this.triangles.Slice(0, n).CopyFrom(result.Slice(0, n));
            
            result.Dispose();
        }

        public void Dispose() {
            plainShape.Dispose();
            triangles.Dispose();
            length.Dispose();
        }
    }
}