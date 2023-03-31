using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOX
{
    public class FOVDrawer : MonoBehaviour
    {
        // ////////////////////////////////////// VARIABLES
        [SerializeField] float viewRadius;
        [SerializeField] float viewAngle;
        [SerializeField] float meshResolution;

        public LayerMask obstacleLayer;
        public LayerMask detectableLayer;

        public List<Transform>visibleTargets = new List<Transform>();

        [SerializeField] public bool isDetecting;

        [SerializeField] Vector3[] vertices;
        [SerializeField] int[] triangles;
        [SerializeField] MeshFilter meshFilter;
        Mesh mesh;
        // ////////////////////////////////////// VARIABLES

        // ////////////////////////////////////// EVENTS
        void Start()
        {
            StartCoroutine(DelayedFindTargets(0.2f));
            intializeFovMesh();
        }

        private void LateUpdate()
        {
            DrawFOV();
        }

        // ////////////////////////////////////// EVENTS


        // ////////////////////////////////////// METHODS
        // Detection

        private void FindTargets()
        {
            visibleTargets.Clear();
            isDetecting = false;

            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, detectableLayer);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleLayer))
                    {
                        visibleTargets.Add(target);
                        isDetecting = true;
                    }
                }
            }

        }


        // FOV Drawing
        void intializeFovMesh()
        {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            mesh.name = "fov";
            mesh.MarkDynamic();
        }

        void DrawFOV()
        {
            int steps = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / steps;
            List<Vector3> viewPoints = new List<Vector3>();

            for (int i = 0; i < steps; i++)
            {
                float angle = transform.eulerAngles.y - (viewAngle / 2) + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);
                viewPoints.Add(newViewCast.point);
            }

            var vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];

            int[] trianglePoints = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    trianglePoints[i * 3] = 0;
                    trianglePoints[i * 3 + 1] = i + 1;
                    trianglePoints[i * 3 + 2] = i + 2;
                }
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = trianglePoints;
            mesh.RecalculateNormals();
        }

        private ViewCastInfo ViewCast(float providedAngle)
        {
            Vector3 dir = DirFromAngle(providedAngle);
            RaycastHit rayCastHit;
            
            if(Physics.Raycast(transform.position, dir, out rayCastHit, viewRadius, obstacleLayer))
            {
                return new ViewCastInfo() { hit = true, point = rayCastHit.point };
            }
            else
            {
                return new ViewCastInfo() { hit = false, point = transform.position + dir * viewRadius };
            }
        }

        public Vector3 DirFromAngle(float angleInDegrees)
        {
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
        // ////////////////////////////////////// METHODS


        // ////////////////////////////////////// COROUTINES
        IEnumerator DelayedFindTargets(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindTargets();
            }
        }

        // ////////////////////////////////////// COROUTINES


    }

    // Custom Class
    public class ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
    }
}

