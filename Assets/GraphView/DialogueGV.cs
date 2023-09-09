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
    public BaseNode StartNode { get; private set; }
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
            StartNode = NodeFactory.CreateNode("StartNode",new Vector2(0, 250),m_tree);
            AddElement(StartNode);
        }
        else
        {
            Debug.Log("Load");
            // create nodes
            m_tree.Dialogues.ForEach((nodeData) => AddElement(NodeFactory.LoadNode(nodeData)));
            // create edges
            nodes.ForEach((node) => {
                if(node is BaseNode baseNode)
                {
                    if(baseNode.NodeData is SingleOutputNodeData)
                    {
                        node.in
                        Edge edge = new();
                        edge.input.ConnectTo()                    }
                }
            });
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
            actionEvent => AddElement(NodeFactory.CreateNode("DialogueNode", actionEvent.eventInfo.localMousePosition,m_tree)));
        evt.menu.AppendAction($"Create ConditionNode",
            actionEvent => AddElement(NodeFactory.CreateNode("ChoicesNode", actionEvent.eventInfo.localMousePosition,m_tree)));
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
                    if(startNode.NodeData is not SingleOutputNodeData startNodeData)
                        throw new Exception("startNode.NodeData is not SingleOutputNodeData");

                    if (InputNode.userData is not BaseNode inputBaseNode)
                        throw new Exception("OutputNode.userData is not BaseNode outputBaseNode");

                    if (inputBaseNode.NodeData == null)
                        throw new Exception("outputBaseNode.NodeData == null");

                    startNodeData.OutputNodeData = inputBaseNode.NodeData;
                    
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
        HashSet<GraphElement> set = new();
        StartNode.CollectElements(set, (element) => true);
        Debug.Log(set.Count);
        foreach(Edge item in set)
        {
            Debug.Log((item.input.node.userData as DialogueBaseNode).title);
        }
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
    public static BaseNode CreateNode(string typeName,Vector2 position,DialogueTree dialogueTree)
    {
        BaseNode node = Activator.CreateInstance(Type.GetType(typeName)) as BaseNode;
        node.Initialize(position,dialogueTree);
        node.Draw();

        return node;
    }

    public static BaseNode LoadNode(GVNodeData nodeData)
    {
        BaseNode node = Activator.CreateInstance(Type.GetType(nodeData.NodeType)) as BaseNode;
        node.LoadNodeData(nodeData);
        node.Draw();

        return node;
    }
}
