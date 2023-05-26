using System;
using iShape.Geometry;
using iShape.Geometry.Container;
using Unity.Collections;
using UnityEngine;

namespace Source {

    public struct TestShape {
        public Vector2[] hull;
        public Vector2[][] holes;

        public PlainShape ToPlainShape(IntGeom iGeom, Allocator allocator) {
            var iHull = iGeom.Int(hull);

            IntShape iShape;
            if (holes != null && holes.Length > 0) {
                var iHoles = iGeom.Int(holes);
                iShape = new IntShape(iHull, iHoles);
            } else {
                iShape = new IntShape(iHull, Array.Empty<IntVector[]>());
            }

            var pShape = new PlainShape(iShape, allocator);

            return pShape;
        }

    }

    public class TriangulationTests {
        
        public static TestShape[] Data = new TestShape[] {
            new TestShape() {
                hull = new Vector2[] {
                    new Vector2(-2.0f, -2.0f),
                    new Vector2(-2.0f, 2.0f),
                    new Vector2(2.0f, 2.0f),
                    new Vector2(2.0f, -2.0f)
                }
            },
            new TestShape() {
                hull = new Vector2[] {
                    new Vector2( 10.0f,  10.0f),
                    new Vector2( 10.0f,  -5.0f),
                    new Vector2(  1.0f,   0.0f),
                    new Vector2(  5.0f,  -5.0f),
                    new Vector2(  3.0f,  -5.0f),
                    new Vector2(  5.0f, -10.0f),
                    new Vector2(-10.0f, -10.0f)
                }
            },
            new TestShape() {
                hull = new Vector2[] {
                    new Vector2(0.736572265625f, -0.095703125f),
                    new Vector2(0.73583984375f, -0.0986328125f),
                    new Vector2(0.740234375f, -0.1005859375f),
                    new Vector2(0.739013671875f, -0.10400390625f),
                    new Vector2(0.740234375f, -0.102783203125f),
                    new Vector2(0.7412109375f, -0.105224609375f),
                    new Vector2(0.732177734375f, -0.1015625f),
                    new Vector2(0.736572265625f, -0.1044921875f),
                    new Vector2(0.73388671875f, -0.10302734375f),
                    new Vector2(0.734619140625f, -0.10595703125f),
                    new Vector2(0.730224609375f, -0.105712890625f),
                    new Vector2(0.730224609375f, -0.109130859375f),
                    new Vector2(0.72509765625f, -0.109619140625f),
                    new Vector2(0.725830078125f, -0.10791015625f),
                    new Vector2(0.722412109375f, -0.108154296875f),
                    new Vector2(0.723876953125f, -0.10546875f),
                    new Vector2(0.720947265625f, -0.105712890625f),
                    new Vector2(0.72314453125f, -0.104248046875f),
                    new Vector2(0.722412109375f, -0.10205078125f),
                    new Vector2(0.71533203125f, -0.099853515625f),
                    new Vector2(0.714111328125f, -0.10302734375f),
                    new Vector2(0.7119140625f, -0.10009765625f),
                    new Vector2(0.714599609375f, -0.1005859375f),
                    new Vector2(0.715087890625f, -0.097412109375f),
                    new Vector2(0.7197265625f, -0.0966796875f),
                    new Vector2(0.724853515625f, -0.100341796875f),
                    new Vector2(0.734130859375f, -0.099853515625f)
                }
            },
        }; 

    }

}