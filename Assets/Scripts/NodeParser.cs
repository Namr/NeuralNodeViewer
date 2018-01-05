using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NodeParser : MonoBehaviour
{

    char[] delimiterChars = {'	', '	', ' '};
    public Transform nodeTemplate;
    public Transform connectionTemplate;

    public string filename = "Node_AAL116.node";


    public float threshold;
    public float thresholdMax;

    List<string> VNodes = new List<string>();
    List<Transform> Nodes = new List<Transform>();
    int numNodes;

    List<string> NodeConnections = new List<string>();
    List<List<string[]>> animatedList = new List<List<string[]>>();
    List<Transform> connections = new List<Transform>();

    public Transform textTransform;
    public Transform IsolationTable;

    Text text;
    public Slider thresholdSlider;

    public bool isDynamic = true;
    public bool isBinary = false;

    List<int[]> ConnectionDataList = new List<int[]>(); //stores the data for which nodes a connection is connected to

    public bool isIsolating = false;
    public int isolatedNode;
    public bool NeedsUpdate = false;

    public int currentFrame;
    int lastFrame;

    public bool doneLoading = false;

    // Use this for initialization
    void Start()
    {
        //load in settings from ini
        INIParser ini = new INIParser();
        ini.Open("config.ini");

        isBinary =  int.Parse(ini.ReadValue("ConnectionData", "IsBinary", "0")) == 0 ? false : true;
        numNodes = int.Parse(ini.ReadValue("NodeData", "NodeCount", "0"));
        string nodePath = ini.ReadValue("NodeData", "NodeDataLoc", "0");
        

        isDynamic = int.Parse(ini.ReadValue("ConnectionData", "IsDynamic", "0")) == 0 ? false : true;
        string connectionPath = ini.ReadValue("ConnectionData", "ConnectionDataLoc", "0");
        thresholdMax = int.Parse(ini.ReadValue("ConnectionData", "ThresholdMaximum", "0")); 
        int numFrames = int.Parse(ini.ReadValue("ConnectionData", "FrameCount", "0"));


        thresholdSlider.value = 0.5f;
        text = textTransform.GetComponent<Text>();
        ParseNodes(nodePath);

        thresholdSlider.transform.parent.gameObject.SetActive(!isBinary);
        thresholdSlider.maxValue = thresholdMax;

        thresholdSlider.value = thresholdMax / 2;

        if(isDynamic)
        {
            for (int i = 0; i < numNodes; i++)
            {
                for (int y = 0; y < numNodes; y++)
                {
                    Transform connection = (Transform)Instantiate(connectionTemplate, new Vector3(0, 0, 0), Quaternion.identity);
                    connection.parent = this.transform;
                    connection.gameObject.SetActive(false);
                    connections.Add(connection);
                }
            }
            for (int i = 1; i < numFrames; i++)
            {
                animatedList.Add(parseDynamicConnections(connectionPath + "/" + i.ToString(), numNodes, i - 1));
            }
            doneLoading = true;
        }
        else
        {
            ParseStaticConnections(connectionPath);
            doneLoading = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Threshold: " + threshold.ToString();
        threshold = thresholdSlider.value;
        if ( (lastFrame != currentFrame || NeedsUpdate) && isDynamic)
        {
            int nodeCount = 0;
            int connectionNumber = 0;
            int IsolatedConnections = 1;
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
                        Vector3 connectionDistance = Nodes[nodeCount].localPosition - Nodes[Connectioncount].localPosition;
                        connections[connectionNumber].localPosition = Nodes[nodeCount].localPosition;
                        connections[connectionNumber].localScale = new Vector3(connections[Connectioncount].localScale.x, connections[Connectioncount].localScale.y, connectionDistance.magnitude * 1.89f);
                        connections[connectionNumber].LookAt(Nodes[Connectioncount].position);
                        if (isBinary)
                        {
                            connections[connectionNumber].GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.red, 1.0f);
                        }
                        else
                        {
                            connections[connectionNumber].GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.red, map(float.Parse(s), 0, thresholdMax, 0, 1));
                        }
                        connections[connectionNumber].name = float.Parse(s).ToString();
                        connections[connectionNumber].gameObject.SetActive(true);
                        if(isIsolating)
                        {
                            if (nodeCount != isolatedNode && Connectioncount != isolatedNode)
                            {
                                connections[connectionNumber].gameObject.SetActive(false);
                            }
                            else
                            {
                                if(IsolatedConnections <= IsolationTable.childCount - 2)
                                {
                                    IsolatedConnections++;
                                    IsolationTable.gameObject.SetActive(true);
                                    IsolationTable.GetChild(IsolatedConnections).GetComponent<Text>().text = Nodes[Connectioncount].name;
                                    IsolatedConnections++;
                                    IsolationTable.GetChild(IsolatedConnections).GetComponent<Text>().text = float.Parse(s).ToString();
                                }
                            }
                        }
                        else
                        {
                            IsolationTable.gameObject.SetActive(false);
                        }
                        connectionNumber++;
                    }
                    Connectioncount++;
                }
                nodeCount++;
            }
            NeedsUpdate = false;
        }
        else if(!isDynamic && NeedsUpdate)
        {
            int nodeCount = 0;
            int connectionNumber = 0;
            int IsolatedConnections = 1;
            foreach (string nodeConnection in NodeConnections)
            {
                string[] properties = nodeConnection.Split(delimiterChars);
                int Connectioncount = 0;
                foreach (string s in properties)
                {
                    if (s != string.Empty && ((isBinary && int.Parse(s) == 1) || (!isBinary && float.Parse(s) > threshold)))
                    {
                        connections[connectionNumber].gameObject.SetActive(true);
                        if (isIsolating)
                        {
                            if (nodeCount != isolatedNode && Connectioncount != isolatedNode)
                            {
                                connections[connectionNumber].gameObject.SetActive(false);
                            }
                            else
                            {
                                if (IsolatedConnections <= IsolationTable.childCount - 2)
                                {
                                    IsolatedConnections++;
                                    IsolationTable.gameObject.SetActive(true);
                                    IsolationTable.GetChild(IsolatedConnections).GetComponent<Text>().text = Nodes[Connectioncount].name;
                                    IsolatedConnections++;
                                    IsolationTable.GetChild(IsolatedConnections).GetComponent<Text>().text = float.Parse(s).ToString();
                                }
                            }
                        }
                    }
                    else
                    {
                        if(connections[connectionNumber] != null)
                        {
                            connections[connectionNumber].gameObject.SetActive(false);
                        }
                    }
                    connectionNumber++;
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
                if (!line.StartsWith("#"))
                {
                    VNodes.Add(line);
                }
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
            //size = float.Parse(properties[4], CultureInfo.InvariantCulture.NumberFormat) * 2;
            size = 3;
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
        nodeParent.transform.localPosition = new Vector3(0, 0, 0);
    }

    void ParseStaticConnections(string file)
    {

        using (StreamReader reader = new StreamReader(file))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.StartsWith("#"))
                {
                    NodeConnections.Add(line);
                }
            }
        }

        int nodeCount = 0;
        foreach (string nodeConnection in NodeConnections)
        {
            string[] properties = nodeConnection.Split(delimiterChars);
            int Connectioncount = 0;
            foreach (string s in properties)
            {
                if (s != string.Empty && float.Parse(s) > 0)
                {
                    Transform connection = (Transform)Instantiate(connectionTemplate, Nodes[nodeCount].position, Quaternion.identity);
                    Vector3 connectionDistance = Nodes[nodeCount].position - Nodes[Connectioncount].position;
                    connection.position = Nodes[nodeCount].position;
                    if(isBinary)
                    {
                        connection.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.red, 1.0f);
                    }
                    else
                    {
                        connection.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.red, map(float.Parse(s), 0, thresholdMax, 0, 1));
                    }
                    connection.localScale = new Vector3(connection.localScale.x, connection.localScale.y, connectionDistance.magnitude);
                    connection.LookAt(Nodes[Connectioncount].position);
                    connection.parent = this.transform;
                    connections.Add(connection);
                    ConnectionDataList.Add(new int[] { nodeCount, Connectioncount });
                    //DrawLine(Nodes[nodeCount].position, Nodes[Connectioncount].position, Color.black, 0.5f);
                }
                else
                {
                    connections.Add(null);
                }
                Connectioncount++;
            }
            nodeCount++;
        }
    }

    List<string[]> parseDynamicConnections(string filepath, int size, int frameNumber)
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

        return AnimatedNodeConnections;
    }

    public void NeedsAnUpdate()
    {
        NeedsUpdate = true;
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
