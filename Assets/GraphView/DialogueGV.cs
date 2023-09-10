using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;

public class DialogueGV : GraphView
{
    DialogueTree m_tree;
    public GraphViewNode StartNode { get; private set; }
    readonly string assetPath = "Assets/DialogueLine.asset";

    public DialogueGV()
    {
        AddManipulator();
        AddBackground();
        AddStyle();

        m_tree = AssetDatabase.LoadAssetAtPath<DialogueTree>(assetPath);
        if (m_tree == null)
        {
            m_tree = ScriptableObject.CreateInstance<DialogueTree>();
            AssetDatabase.CreateAsset(m_tree, assetPath);
            // init node
            StartNode = NodeFactory.CreateNode<StartNode>(new Vector2(0, 250),m_tree);
            AddElement(StartNode);
        }
        else
        {
            Debug.Log("Load");
            // create nodes
            m_tree.Dialogues.ForEach((nodeData) => AddElement(NodeFactory.LoadNode(nodeData)));
            // create edges
            /*
            nodes.ForEach((node) => {
                if(node.userData is GVNodeData nodeData)
                {
                    foreach(var child in nodeData.GetChildren())
                    {
                        Node childNode = GetNodeByGuid(child.Id);
                        node.outputContainer
                        if(childNode != null)
                        {
                            Port port = new();
                            port.ConnectTo
                        }
                    }
                }
            });*/
        }

        graphViewChanged += OnGraphViewChange;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compartiblePorts = new();

        ports.ForEach(port =>
        {
            if (startPort == port)                              // can't connect to self
                return;
            if (startPort.node == port.node)                    // can't connect to self node
                return;
            if (startPort.direction == port.direction)          // can't connect to same dir
                return;

            compartiblePorts.Add(port);
        });

        return compartiblePorts;
    }

    void AddManipulator()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());        // need to add before RectangleSelector()
        this.AddManipulator(new RectangleSelector());

        this.AddManipulator(SaveContextualMenu());
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        
        evt.menu.AppendAction($"Create DialogueNode",
            actionEvent => AddElement(NodeFactory.CreateNode<DialogueNode>(actionEvent.eventInfo.localMousePosition,m_tree)));
        evt.menu.AppendAction($"Create ConditionNode",
            actionEvent => AddElement(NodeFactory.CreateNode<ChoicesNode>(actionEvent.eventInfo.localMousePosition,m_tree)));
        
    }

    IManipulator SaveContextualMenu()
    {
        return new ContextualMenuManipulator(ev =>
        {
            ev.menu.AppendAction("Save Graph",actionEvent => SaveGraph());
        });
    }

    GraphViewChange OnGraphViewChange(GraphViewChange changes)
    {
        Debug.Log("Changed");
        if (changes.edgesToCreate != null)
        {
            foreach (var edge in changes.edgesToCreate)
            {
                Node InputNode = edge.input.node;
                Node OutputNode = edge.output.node;
                Debug.Log($"{OutputNode.title} -> {InputNode.title}");

                if (OutputNode.userData is StartNode startNode)
                {
                    if (InputNode.userData is not GVNodeData inputNode)
                        throw new Exception("OutputNode.userData is not BaseNode outputBaseNode");

                    if (inputNode == null)
                        throw new Exception("outputBaseNode.NodeData == null");

                    Debug.Log("a");
                    startNode.AddChild(inputNode);
                    
                }
                else
                {
                    Debug.Log("aaaa");
                }
            }
        }

        if (changes.elementsToRemove != null)
        {
            foreach (var element in changes.elementsToRemove)
            {
                if (element.GetType() == typeof(Edge))
                {
                    Debug.Log("Edge was removed.");
                }
                if (element.GetType() == typeof(Node))
                {
                    Debug.Log("Node was removed.");
                }
            }
        }

        if (changes.movedElements != null)
        {

            Debug.Log("something moved");

        }

        return changes;
    }

    void SaveGraph()
    {
        Debug.Log("Save");
        /*
        HashSet<GraphElement> set = new();
        StartNode.CollectElements(set, (element) => true);
        Debug.Log(set.Count);
        foreach(Edge item in set)
        {
            Debug.Log(item.input.node.title);
        }
        */
        Debug.Log(GetPortByGuid("sos").node.title);

    }

    void AddBackground()
    {
        GridBackground gridBackground = new();
        gridBackground.StretchToParentSize();
        Insert(0,gridBackground);
    }

    void AddStyle()
    {
        StyleSheet GrahpviewStyleSheet = (StyleSheet) EditorGUIUtility.Load("DialogueGVStyle.uss");
        StyleSheet nodeStyleSheet = (StyleSheet) EditorGUIUtility.Load("NodeStyle.uss");
        styleSheets.Add(GrahpviewStyleSheet);
        styleSheets.Add(nodeStyleSheet);
    }
}

public static class NodeFactory
{
    public static GraphViewNode CreateNode<T>(Vector2 position,DialogueTree dialogueTree) where T:GVNodeData
    {
        var nodeData = ScriptableObject.CreateInstance<T>();
        nodeData.Initialize(position,dialogueTree);

        return nodeData.CreateNode() as GraphViewNode;
    }

    public static GraphViewNode LoadNode(GVNodeData nodeData)
    {
        return nodeData.CreateNode() as GraphViewNode;
    }
}
