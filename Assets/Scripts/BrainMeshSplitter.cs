using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BrainMeshSplitter : MonoBehaviour
{


    public string filename = "BrainMesh_ICBM152_smoothed.nv";
    public Transform BrainMesh;

    char[] delimiterChars = { ' ', '	' };

    // Use this for initialization
    void Start()
    {
        MeshFilter mf = transform.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        using (StreamReader reader = new StreamReader(filename))
        {
            int vertexNumber = int.Parse(reader.ReadLine());

            Vector3[] vertices = new Vector3[vertexNumber];
            string[] rawVertices = new string[vertexNumber];

            Vector3[] posVertices = new Vector3[vertexNumber];
            Vector3[] negVertices = new Vector3[vertexNumber];


            bool[] isPositive = new bool[vertexNumber];
            int[] newIndex = new int[vertexNumber];
            for (int i = 0; i < vertexNumber; i++)
            {
                rawVertices[i] = reader.ReadLine();
                string[] properties = rawVertices[i].Split(delimiterChars);
                vertices[i] = new Vector3(float.Parse(properties[0]), float.Parse(properties[1]), float.Parse(properties[2]));
            }

            if (vertexNumber > 65000)
            {
                int posCount = 0;
                int negCount = 0;
                int count = 0;
                foreach (Vector3 vertex in vertices)
                {
                    if (vertex.x > -3)
                    {
                        posVertices[posCount] = vertex;
                        isPositive[count] = true;
                        newIndex[count] = posCount;
                        posCount++;
                    }
                    else
                    {
                        negVertices[negCount] = vertex;
                        isPositive[count] = false;
                        newIndex[count] = negCount;
                        negCount++;
                    }
                    count++;
                }

                posVertices = posVertices.Where(c => c != Vector3.zero).ToArray();
                negVertices = negVertices.Where(c => c != Vector3.zero).ToArray();

                Transform brainChild = (Transform)Instantiate(BrainMesh, new Vector3(0, 0, 0), Quaternion.identity);
                Mesh childMesh = new Mesh();
                brainChild.parent = this.transform.parent;
                brainChild.GetComponent<MeshFilter>().mesh = childMesh;

                mesh.vertices = posVertices;
                childMesh.vertices = negVertices;

                int triNumber = int.Parse(reader.ReadLine());
                int[] posTriangles = new int[triNumber * 3];
                int[] negTriangles = new int[triNumber * 3];
                int posTriCount = 0;
                int negTriCount = 0;
                for (int i = 0; i < triNumber; i++)
                {
                    string triData = reader.ReadLine();
                    string[] properties = triData.Split(delimiterChars);

                    int index0 = int.Parse(properties[0]) - 1;
                    int index1 = int.Parse(properties[1]) - 1;
                    int index2 = int.Parse(properties[2]) - 1;

                    if(isPositive[index0] && isPositive[index1] && isPositive[index2])
                    {
                        posTriangles[posTriCount] = newIndex[index0];
                        posTriCount++;
                        posTriangles[posTriCount] = newIndex[index1];
                        posTriCount++;
                        posTriangles[posTriCount] = newIndex[index2];
                        posTriCount++;
                    }
                    else if (!isPositive[index0] && !isPositive[index1] && !isPositive[index2])
                    {
                        negTriangles[negTriCount] = newIndex[index0];
                        negTriCount++;
                        negTriangles[negTriCount] = newIndex[index1];
                        negTriCount++;
                        negTriangles[negTriCount] = newIndex[index2];
                        negTriCount++;
                    }
                }
                mesh.triangles = posTriangles;
                childMesh.triangles = negTriangles;
            }
            else
            {
                mesh.vertices = vertices;
                int triNumber = int.Parse(reader.ReadLine());
                int[] triangles = new int[triNumber * 3];
                int triCount = 0;
                for (int i = 0; i < triNumber; i++)
                {
                    string triData = reader.ReadLine();
                    string[] properties = triData.Split(delimiterChars);

                    triangles[triCount] = int.Parse(properties[0]) - 1;
                    triCount++;
                    triangles[triCount] = int.Parse(properties[1]) - 1;
                    triCount++;
                    triangles[triCount] = int.Parse(properties[2]) - 1;
                    triCount++;
                }
                mesh.triangles = triangles;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
