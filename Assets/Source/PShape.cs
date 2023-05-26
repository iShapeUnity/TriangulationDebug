using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

namespace Source {

    public class PShape : MonoBehaviour {

        private MeshFilter meshFilter;
        private Mesh mesh;
        public Text sceneText;
        public TriangulationJobHandler triangulationJobHandler;

        private int state = 0;

        public void ButtonClick() {
            switch (state) {
                case 0:
                    state = 1;
                    sceneText.text = "Delaunay";
                    break;
                default:
                    state = 0;
                    sceneText.text = "Monotone";
                    break;
            }
        }

        private void Awake() {
            Application.targetFrameRate = 60;
            this.meshFilter = gameObject.GetComponent<MeshFilter>();
            this.mesh = new Mesh();
            this.mesh.MarkDynamic();
            this.meshFilter.mesh = mesh;
            this.ButtonClick();
        }

        private float k = 0.0f;
        private float d = 0.005f;

        private void Update() {
            if (k < -0.5f) {
                d = 0.005f;
            } else if (k > 0.5f) {
                d = -0.005f;
            }

            k += d;
            this.UpdateShape(512, 4f);
        }

        private void LateUpdate() {
            if (triangulationJobHandler != null) {
                var shapeMesh = triangulationJobHandler.Complete(Allocator.Temp);
                shapeMesh.FillAndDispose(mesh);
                triangulationJobHandler = null;
            }
        }

        private void UpdateShape(int n, float radius) {
            float da0 = 2f * Mathf.PI / n; 
            float da1 = 16f * Mathf.PI / n;
            float delta = k * radius;

            var hull = new NativeArray<Vector2>(n, Allocator.Temp);

            float a0 = 0f;
            float a1 = 0f;

            for (int i = 0; i < n; ++i) {
                float r = radius + delta * Mathf.Sin(a1);
                float x = r * Mathf.Cos(a0);
                float y = r * Mathf.Sin(a0);
                a0 -= da0;
                a1 -= da1;
                hull[i] = new Vector2(x, y);
            }

            var hole = new NativeArray<Vector2>(n, Allocator.Temp);

            for (int i = 0; i < n; ++i) {
                var v = 0.5f * hull[n - i - 1];
                hole[i] = v;
            }

            switch (state) {
                case 0:
                    triangulationJobHandler = new TriangulationJobHandler();
                    triangulationJobHandler.Invoke(hull, hole, false);
                    break;
                default:
                    triangulationJobHandler = new TriangulationJobHandler();
                    triangulationJobHandler.Invoke(hull, hole, true);
                    break;
            }

            hull.Dispose();
            hole.Dispose();
        }

    }

}