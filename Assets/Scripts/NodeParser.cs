using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NodeParser : MonoBehaviour
{

    char[] delimiterChars = { '	', '	' };
    public Transform nodeTemplate;
    public Transform connectionTemplate;

    public string filename = "Node_AAL116.node";


    public float threshold;

    List<List<float>> thresholdList = new List<List<float>>();

    List<List<int[]>> ConnectionDataList = new List<List<int[]>>(); //stores the data for which nodes a connection is connected to

    List<string> VNodes = new List<string>();
    List<Transform> Nodes = new List<Transform>();

    List<string> NodeConnections = new List<string>();
    List<List<string[]>> animatedList = new List<List<string[]>>();
    List<Transform> connections = new List<Transform>();

    public Transform textTransform;
    Text text;
    public Slider thresholdSlider;

    public bool isIsolating = false;
    public int isolatedNode;

    public int currentFrame;
    int lastFrame;
    // Use this for initialization
    void Start()
    {
        thresholdSlider.value = 0.5f;
        text = textTransform.GetComponent<Text>();
        ParseNodes(filename);

        for (int i = 0; i < 116; i++)
        {
            for (int y = 0; y < 116; y++)
            {
                Transform connection = (Transform)Instantiate(connectionTemplate, new Vector3(0, 0, 0), Quaternion.identity);
                connection.parent = this.transform;
                connection.gameObject.SetActive(false);
                connections.Add(connection);
            }
        }
        for (int i = 1; i < 51; i++)
        {
            thresholdList.Add(new List<float>());
            ConnectionDataList.Add(new List<int[]>());
            animatedList.Add(parseAnimatedConnections("Functional Dynamic Data/" + i.ToString(), 116, i - 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Threshold: " + threshold.ToString();
        threshold = thresholdSlider.value;
        if (lastFrame != currentFrame)
        {
            int nodeCount = 0;
            int connectionNumber = 0;
            //reset connections
            foreach(Transform connection in connections)
            {
                connection.gameObject.SetActive(false);
            }

            foreach (string[] properties in animatedList[currentFrame])
            {
                int Connectioncount = 0;
                foreach (string s in properties)
                {
                    if (float.Parse(s) > threshold)
                    {   
                        Vector3 connectionDistance = Nodes[nodeCount].position - Nodes[Connectioncount].position;
                        connections[connectionNumber].position = Nodes[nodeCount].position;
                        connections[connectionNumber].localScale = new Vector3(connections[Connectioncount].localScale.x, connections[Connectioncount].localScale.y, connectionDistance.magnitude * 1.89f);
                        connections[connectionNumber].LookAt(Nodes[Connectioncount].position);
                        connections[connectionNumber].GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.red, float.Parse(s));
                        connections[connectionNumber].name = float.Parse(s).ToString();
                        connections[connectionNumber].gameObject.SetActive(true);
                        if (isIsolating && nodeCount != isolatedNode && Connectioncount != isolatedNode)
                        {
                            connections[connectionNumber].gameObject.SetActive(false);
                        }
                        connectionNumber++;
                    }
                    Connectioncount++;
                }
                nodeCount++;
            }
        }

        lastFrame = currentFrame;
    }

    void ParseNodes(string file)
    {
        GameObject nodeParent = new GameObject("Node Parent");
        nodeParent.transform.parent = this.transform;
        using (StreamReader reader = new StreamReader(file))
        {

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                VNodes.Add(line);
            }
        }
        int nodeIndex = 0;
        foreach (string vNode in VNodes)
        {
            string[] properties = vNode.Split(delimiterChars);
            float x, y, z, color, size;
            x = float.Parse(properties[0], CultureInfo.InvariantCulture.NumberFormat);
            y = float.Parse(properties[1], CultureInfo.InvariantCulture.NumberFormat);
            z = float.Parse(properties[2], CultureInfo.InvariantCulture.NumberFormat);
            color = float.Parse(properties[3], CultureInfo.InvariantCulture.NumberFormat);
            size = float.Parse(properties[4], CultureInfo.InvariantCulture.NumberFormat) * 2;
            Transform node = (Transform)Instantiate(nodeTemplate, new Vector3(x, y, z), Quaternion.identity);
            float vectorScale = node.localScale.x + size;
            node.localScale = new Vector3(vectorScale, vectorScale, vectorScale);
            node.name = properties[5] + nodeIndex.ToString();
            node.parent = nodeParent.transform;
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
                case 6:
                    node.gameObject.GetComponent<Renderer>().material.color = Color.gray;
                    break;
            }
            nodeIndex++;
            Nodes.Add(node);
        }
    }

    void ParseConnections()
    {

        using (StreamReader reader = new StreamReader("Edge_AAL90_Binary.edge"))
        {
            string line;
            line = reader.ReadLine();
            //Debug.Log(line);
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
            foreach (string s in properties)
            {
                if (int.Parse(s) == 1)
                {
                    Transform connection = (Transform)Instantiate(connectionTemplate, Nodes[nodeCount].position, Quaternion.identity);
                    Vector3 connectionDistance = Nodes[nodeCount].position - Nodes[Connectioncount].position;
                    connection.position = Nodes[nodeCount].position;
                    connection.localScale = new Vector3(connection.localScale.x, connection.localScale.y, connectionDistance.magnitude);
                    connection.LookAt(Nodes[Connectioncount].position);
                    connection.parent = this.transform;
                    //DrawLine(Nodes[nodeCount].position, Nodes[Connectioncount].position, Color.black, 0.5f);
                }
                Connectioncount++;
            }
            nodeCount++;
        }
    }

    List<string[]> parseAnimatedConnections(string filepath, int size, int frameNumber)
    {
        List<string[]> AnimatedNodeConnections = new List<string[]>();
        size--;

        using (StreamReader reader = new StreamReader(filepath))
        {
            string line;
            line = reader.ReadLine();

            int count = 0;
            string[] properties = new string[size];

            while ((line = reader.ReadLine()) != null)
            {
                if (count < size)
                {
                    properties[count] = line;
                    count++;
                }
                else
                {
                    count = 0;
                    AnimatedNodeConnections.Add(properties);
                    // properties = new string[size];
                }
            }
        }

        int nodeCount = 0;
        foreach (string[] properties in AnimatedNodeConnections)
        {
            int Connectioncount = 0;
            foreach (string s in properties)
            {
                if (float.Parse(s) > 0)
                {
                    thresholdList[frameNumber].Add(float.Parse(s));
                    ConnectionDataList[frameNumber].Add(new int[] { nodeCount, Connectioncount });
                }
                Connectioncount++;
            }
            nodeCount++;
        }

        return AnimatedNodeConnections;
    }
}
