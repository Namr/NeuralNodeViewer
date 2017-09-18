using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrainMeshSlicing : MonoBehaviour {
    public Transform BrainMesh;
    public Transform Slicer;
    public Pointer pointer;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(pointer.pointerMode == Pointer.Mode.Slicing && OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            Slice();
        }
	}
    
    public void Slice()
    {
        //get your components
        MeshFilter mf = transform.GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;

        //get number of and data for the vertices in the mesh
        int vertexNumber = mesh.vertexCount;
        Vector3[] vertices = mesh.vertices;

        //these are the arrays that will hold the vertices after their split
        Vector3[] posVertices = new Vector3[vertexNumber];
        Vector3[] negVertices = new Vector3[vertexNumber];

        //for each index(vertex) is it above or bellow the bound set
        bool[] isPositive = new bool[vertexNumber];
        //this varible stores new the index for every vertex in the old file
        int[] newIndex = new int[vertexNumber];

        int posCount = 0;
        int negCount = 0;
        int count = 0;
        //go through every vertex and see which child mesh it belongs in and place it there, also keep track of the indexes
        foreach (Vector3 vertex in vertices)
        {
            if (Vector3.Dot(vertex,Slicer.position) > 0)
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

        //init child mesh
        Transform brainChild = (Transform)Instantiate(BrainMesh, new Vector3(0, 0, 0), Quaternion.identity);
        Mesh childMesh = new Mesh();
        brainChild.parent = this.transform.parent;
        brainChild.GetComponent<MeshFilter>().mesh = childMesh;
        brainChild.GetComponent<BrainMeshSlicing>().Slicer = Slicer;
        brainChild.GetComponent<BrainMeshSlicing>().pointer = pointer;
        /*
        mesh.vertices = posVertices;
        childMesh.vertices = negVertices;
        
        int triNumber = mesh.triangles.Length;
        int[] posTriangles = new int[triNumber * 3];
        int[] negTriangles = new int[triNumber * 3];
        int posTriCount = 0;
        int negTriCount = 0;
        int normalTriCount = 0;
        for (int i = 0; i < triNumber; i++)
        {
            //the original index of the three vertices of the triangles
            int index0 = mesh.triangles[normalTriCount];
            normalTriCount++;
            int index1 = mesh.triangles[normalTriCount];
            normalTriCount++;
            int index2 = mesh.triangles[normalTriCount];
            normalTriCount++;

            //if all vertices of the triangle are positive, find their new index and make a traingle
            if (isPositive[index0] && isPositive[index1] && isPositive[index2])
            {
                posTriangles[posTriCount] = newIndex[index0];
                posTriCount++;
                posTriangles[posTriCount] = newIndex[index1];
                posTriCount++;
                posTriangles[posTriCount] = newIndex[index2];
                posTriCount++;
            }
            //if all vertices of the triangle are negative, find their new index and make a traingle
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
        
        //populate the meshes
        mesh.triangles = posTriangles;
        childMesh.triangles = negTriangles;
        */
    }
}
