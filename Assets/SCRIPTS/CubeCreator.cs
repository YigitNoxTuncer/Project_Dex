using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOX
{
    public class CubeCreator : MonoBehaviour
    {
        [SerializeField]List<Transform> transformPoints;

        [SerializeField]Vector3[] vertices;
        [SerializeField]int[] triangles;
        [SerializeField]MeshFilter meshFilter;
        Mesh mesh;

        private void Start()
        {
            intializeMesh();
        }

        private void LateUpdate()
        {
            meshDrawer();
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
            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 3;
            //Face 2
            triangles[6] = 4;
            triangles[7] = 5;
            triangles[8] = 6;
            triangles[9] = 6;
            triangles[10] = 5;
            triangles[11] = 7;
            //Face 3
            triangles[12] = 8;
            triangles[13] = 9;
            triangles[14] = 11;
            triangles[15] = 10;
            triangles[16] = 8;
            triangles[17] = 11;
            //Face 4
            triangles[18] = 12;
            triangles[19] = 13;
            triangles[20] = 15;
            triangles[21] = 14;
            triangles[22] = 12;
            triangles[23] = 15;
            //Face 5
            triangles[24] = 18;
            triangles[25] = 17;
            triangles[26] = 16;
            triangles[27] = 19;
            triangles[28] = 18;
            triangles[29] = 16;
            //Face 6
            triangles[30] = 20;
            triangles[31] = 21;
            triangles[32] = 22;
            triangles[33] = 23;
            triangles[34] = 20;
            triangles[35] = 22;



        }

        void meshDrawer()
        {

            SetVertices();
            SetTriangles();

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        void intializeMesh()
        {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            mesh.name = "CreatedCube";
            mesh.MarkDynamic();
        }
    }
}

