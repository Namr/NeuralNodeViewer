using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrainMeshSlicing : MonoBehaviour
{
    public Transform BrainMesh;
    public Transform Slicer;
    public Pointer pointer;
    public Transform LatestMoveablePiece;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pointer.pointerMode == Pointer.Mode.Slicing)
        {
            if(pointer.isVR && OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                Slice();
            }
            else if(!pointer.isVR && Input.GetMouseButtonUp(1))
            {
                Slice();
            }
        }
        if(pointer.pointerMode == Pointer.Mode.MoveSliced)
        {
            gameObject.layer = 8;
        }
        else
        {
            gameObject.layer = 2;
        }
    }

    public void Slice()
    {
        //get your components
        MeshFilter mf = transform.GetComponent<MeshFilter>();
        //Mesh mesh = mf.mesh;

        //get number of and data for the vertices in the mesh
        int vertexNumber = mf.mesh.vertexCount;
        Vector3[] vertices = mf.mesh.vertices;

        //these are the arrays that will hold the vertices after their split
        Vector3[] posVertices = new Vector3[vertexNumber];
        Vector3[] negVertices = new Vector3[vertexNumber];

        //for each index(vertex) is it above or bellow the bound set
        bool[] isPositive = new bool[vertexNumber];
        //this varible stores new the index for every vertex in the old file
        int[] newIndex = new int[vertexNumber];
        int[] triangles = mf.mesh.triangles;
        int posCount = 0;
        int negCount = 0;
        int count = 0;
        //go through every vertex and see which child mesh it belongs in and place it there, also keep track of the indexes
        foreach (Vector3 vertex in vertices)
        {
            if (Slicer.transform.InverseTransformPoint(transform.TransformPoint(vertex)).y > 0)
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
        brainChild.name = "BrainMesh";
        brainChild.GetComponent<MeshFilter>().mesh = childMesh;
        brainChild.GetComponent<BrainMeshSlicing>().Slicer = Slicer;
        brainChild.GetComponent<BrainMeshSlicing>().pointer = pointer;
        brainChild.rotation = transform.rotation;

        mf.mesh.Clear();
        childMesh.Clear();
        mf.mesh.vertices = posVertices;
        childMesh.vertices = negVertices;




        List<int> posTriangles = new List<int>();
        List<int> negTriangles = new List<int>();

        for (int i = 0; i < triangles.Length; i+= 0)
        {
            int val1 = i;
            i++;
            int val2 = i;
            i++;
            int val3 = i;
            i++;

            if (isPositive[triangles[val1]] && isPositive[triangles[val2]] && isPositive[triangles[val3]])
            {
                posTriangles.Add(newIndex[triangles[val1]]);
                posTriangles.Add(newIndex[triangles[val2]]);
                posTriangles.Add(newIndex[triangles[val3]]);
            }
            if (!isPositive[triangles[val1]] && !isPositive[triangles[val2]] && !isPositive[triangles[val3]])
            {
                negTriangles.Add(newIndex[triangles[val1]]);
                negTriangles.Add(newIndex[triangles[val2]]);
                negTriangles.Add(newIndex[triangles[val3]]);
            }
        }

        mf.mesh.triangles = posTriangles.ToArray();
        childMesh.triangles = negTriangles.ToArray();
        transform.GetComponent<MeshCollider>().sharedMesh = mf.mesh;
        brainChild.GetComponent<MeshCollider>().sharedMesh = childMesh;

        transform.localScale = transform.localScale;
        transform.localPosition = new Vector3(3.264759f, 0.5278605f, -1.476445f);

        brainChild.localPosition = new Vector3(3.264759f, 0.5278605f, -1.476445f);
        brainChild.localScale = transform.localScale;
    }
}
