using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class NodeParser : MonoBehaviour {

    char[] delimiterChars = {'	', '	'};
    public Transform nodeTemplate;
    public Transform connectionTemplate;
    // Use this for initialization
    void Start()
    {
        List<string> VNodes = new List<string>();
        List<Transform> Nodes = new List<Transform>();

        using (StreamReader reader = new StreamReader("Node_AAL90.node"))
        {
            
            string line;
            line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                VNodes.Add(line); 
            }
        }
        
        foreach(string vNode in VNodes)
        {
            string[] properties = vNode.Split(delimiterChars);
            float x, y, z, color, size;
            x = float.Parse(properties[0], CultureInfo.InvariantCulture.NumberFormat);
            y = float.Parse(properties[1], CultureInfo.InvariantCulture.NumberFormat);
            z = float.Parse(properties[2], CultureInfo.InvariantCulture.NumberFormat);
            color = float.Parse(properties[3], CultureInfo.InvariantCulture.NumberFormat);
            size = float.Parse(properties[4], CultureInfo.InvariantCulture.NumberFormat) * 2;
            Transform node = (Transform) Instantiate(nodeTemplate, new Vector3(x,y,z), Quaternion.identity);
            float vectorScale = node.localScale.x + size;
            node.localScale = new Vector3(vectorScale, vectorScale, vectorScale);
            node.name = properties[5];
            node.parent = this.transform;
            node.tag = "Node";
            switch ((int)color)
            {
                case 1:
                    node.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                case 2:
                    node.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    break;
                case 3:
                    node.gameObject.GetComponent<Renderer>().material.color = Color.blue;
                    break;
                case 4:
                    node.gameObject.GetComponent<Renderer>().material.color = Color.red;
                    break;
                case 5:
                    node.gameObject.GetComponent<Renderer>().material.color = Color.cyan;
                    break;
            }
            Nodes.Add(node);
        }

        List<string> NodeConnections = new List<string>();
       
        using (StreamReader reader = new StreamReader("Edge_AAL90_Binary.edge"))
        {
            string line;
            line = reader.ReadLine();
            Debug.Log(line);
            while ((line = reader.ReadLine()) != null)
            {
                NodeConnections.Add(line);
            }
        }

        int nodeCount = 0;
        foreach (string nodeConnection in NodeConnections)
        {
            string[] properties = nodeConnection.Split(delimiterChars);
            int Connectioncount = 0;
            foreach(string s in properties)
            {
                if(int.Parse(s) == 1)
                {
                    Transform connection = (Transform)Instantiate(connectionTemplate, Nodes[nodeCount].position, Quaternion.identity);
                    Vector3 connectionDistance = Nodes[nodeCount].position - Nodes[Connectioncount].position;
                    connection.position = Nodes[nodeCount].position;
                    connection.localScale = new Vector3(connection.localScale.x, connection.localScale.y, connectionDistance.magnitude);
                    connection.LookAt(Nodes[Connectioncount].position);
                    //DrawLine(Nodes[nodeCount].position, Nodes[Connectioncount].position, Color.black, 0.5f);
                }
                Connectioncount++;
            }
            nodeCount++;
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    void DrawLine(Vector3 start, Vector3 end, Color color, float width)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        myLine.transform.parent = this.transform;
    }
}
