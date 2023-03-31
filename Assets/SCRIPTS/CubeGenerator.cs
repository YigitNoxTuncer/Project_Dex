using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOX
{
    public class CubeGenerator : MonoBehaviour
    {
        [SerializeField] List<Transform> transformPoints;

        [SerializeField] Vector3[] vertices;
        [SerializeField] int[] triangles;
        [SerializeField] MeshFilter meshFilter;
        Mesh mesh;

        private void Start()
        {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            mesh.name = "CreatedCube";
            mesh.MarkDynamic();
            SetVertices();
            SetTriangles();
        }


        private void SetVertices()
        {
            vertices = new Vector3[transformPoints.Count];
            for (int i = 0; i < transformPoints.Count; i++)
            {
                var vertex = transformPoints[i].position;
                vertices[i] = vertex;
            }
        }

        private void SetTriangles()
        {
            triangles = new int[36];

            //Face 1
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 3;
            triangles[4] = 5;
            triangles[5] = 4;


        }
    }
}

