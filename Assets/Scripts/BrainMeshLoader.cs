using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BrainMeshLoader : MonoBehaviour {

    public Transform BrainMesh;
    public string filename = "BrainMesh_ICBM152_smoothed.nv";

    char[] delimiterChars = {' ', '	' };

    // Use this for initialization
    void Start()
    {

        
        using (StreamReader reader = new StreamReader(filename))
        {
            int vertexNumber = int.Parse(reader.ReadLine());
            LoadMesh(this.transform,vertexNumber,0,reader,false);
        }

    }

    void LoadMesh(Transform meshTransform,int vertexNumber, int vertexOffset,StreamReader reader,bool isChild)
    {
        bool isSplitting = false;
        int newVertexNumber = 0;
        if(vertexNumber >= 65000)
        {
            isSplitting = true;
            newVertexNumber = vertexNumber - 65000;
            vertexNumber = 65000;
        }

        MeshFilter mf = meshTransform.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[vertexNumber];

        for (int i = 0; i < vertexNumber; i++)
        {
            string vertexData = reader.ReadLine();
            string[] properties = vertexData.Split(delimiterChars);
            vertices[i] = new Vector3(float.Parse(properties[0]), float.Parse(properties[1]), float.Parse(properties[2]));
        }
        mesh.vertices = vertices;
        int[] Triangles;
        if (isSplitting)
        {
            Transform brainChild = (Transform)Instantiate(BrainMesh, new Vector3(0, 0, 0), Quaternion.identity);
            int[] triangles = LoadChildMesh(brainChild, newVertexNumber, 65000, reader);
            Triangles = triangles;
        }
        else
        {
            int triNumber = int.Parse(reader.ReadLine());
            int[] triangles = new int[triNumber * 3];
            int triCount = 0;
            for (int i = 0; i < triNumber; i++)
            {
                string triData = reader.ReadLine();
                string[] properties = triData.Split(delimiterChars);

                if (int.Parse(properties[0]) > vertexNumber || int.Parse(properties[0]) < vertexOffset)
                    break;

                if (int.Parse(properties[1]) > vertexNumber || int.Parse(properties[1]) < vertexOffset)
                    break;

                if (int.Parse(properties[2]) > vertexNumber || int.Parse(properties[2]) < vertexOffset)
                    break;

                triangles[triCount] = int.Parse(properties[0]) - 1;
                triCount++;
                triangles[triCount] = int.Parse(properties[1]) - 1;
                triCount++;
                triangles[triCount] = int.Parse(properties[2]) - 1;
                triCount++;
            }
            Triangles = triangles;
        }
        
        mesh.triangles = Triangles;
        mf.mesh = mesh;
    }

    int[] LoadChildMesh(Transform meshTransform, int vertexNumber, int vertexOffset, StreamReader reader)
    {
        MeshFilter mf = meshTransform.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[vertexNumber];

        for (int i = 0; i < vertexNumber; i++)
        {
            string vertexData = reader.ReadLine();
            string[] properties = vertexData.Split(delimiterChars);
            vertices[i] = new Vector3(float.Parse(properties[0]), float.Parse(properties[1]), float.Parse(properties[2]));
        }
        mesh.vertices = vertices;

        int triNumber = int.Parse(reader.ReadLine());
        int[] topTriangles = new int[triNumber * 3];
        int triCount = 0;
        for (int i = 0; i < triNumber; i++)
        {
            string triData = reader.ReadLine();
            string[] properties = triData.Split(delimiterChars);

            if (int.Parse(properties[0]) > 65000 || int.Parse(properties[0]) < 0
                || int.Parse(properties[1]) > 65000 || int.Parse(properties[1]) < 0 
                || int.Parse(properties[2]) > 65000 || int.Parse(properties[2]) < 0)
            {
                break;
            }

            topTriangles[triCount] = int.Parse(properties[0]) - 1;
            triCount++;
            topTriangles[triCount] = int.Parse(properties[1]) - 1;
            triCount++;
            topTriangles[triCount] = int.Parse(properties[2]) - 1;
            triCount++;
        }
        int[] myTriangles = new int[triNumber * 3];
        triNumber -= triCount / 3;
        triCount = 0;
        Debug.Log(reader.ReadLine());
        for (int i = 0; i < triNumber; i++)
        {
            string triData = reader.ReadLine();
            if (triData == null)
                break;
            string[] properties = triData.Split(delimiterChars);
            if (int.Parse(properties[0]) < 65000 || int.Parse(properties[1]) < 65000 || int.Parse(properties[2]) < 65000
                || int.Parse(properties[0]) > vertexNumber + 65000|| int.Parse(properties[1]) > vertexNumber + 65000 || int.Parse(properties[2]) > vertexNumber + 65000)
            {
                
            }
            else
            {
                myTriangles[triCount] = int.Parse(properties[0]) - 65001;
                triCount++;
                myTriangles[triCount] = int.Parse(properties[1]) - 65001;
                triCount++;
                myTriangles[triCount] = int.Parse(properties[2]) - 65001;
                triCount++;
            }
        }
        Debug.Log(triCount);
        mesh.triangles = myTriangles;
        mf.mesh = mesh;
        return topTriangles;
    }
    // Update is called once per frame
    void Update ()
    {
		
	}
}
