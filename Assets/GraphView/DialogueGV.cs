using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;
using Graphview.NodeData;

public class DialogueGV : GraphView
{
    public DialogueTree Tree { get; private set; }
    public GraphViewNode StartNode { get; private set; }

    public DialogueGV(string assetPath)
    {
        AddManipulator();
        AddBackground();
        AddStyle();

        LoadAsset(assetPath);

        graphViewChanged += OnGraphViewChange;
    }

    void LoadAsset(string assetPath)
    {
        Tree = AssetDatabase.LoadAssetAtPath<DialogueTree>(assetPath);

        Debug.Log($"Load {assetPath}");
        foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(assetPath))
        {
            if (obj is DialogueTree dialogueTree)
                Tree = dialogueTree;
            else if (obj is GVNodeData nodeData)
                AddElement(NodeFactory.LoadNode(nodeData));
            else
                throw new Exception($"Unexpected asset type. {obj.GetType()}");
        }
        // create edges
        
        foreach(var edgeData in Tree.Edges)
        {
            Port outputPort = GetPortByGuid(edgeData.From);
            Port inputPort = GetPortByGuid(edgeData.To);

            if (outputPort == null)
                throw new NullReferenceException("outputPort not found");
            if (inputPort == null)
                throw new NullReferenceException("inputPort not found");

            Edge edge = outputPort.ConnectTo(inputPort);
            edge.viewDataKey = edgeData.Id;
            AddElement(edge);
        }
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
            actionEvent => AddElement(NodeFactory.CreateNode<DialogueNode>(actionEvent.eventInfo.localMousePosition,Tree)));
        evt.menu.AppendAction($"Create ConditionNode",
            actionEvent => AddElement(NodeFactory.CreateNode<ChoicesNode>(actionEvent.eventInfo.localMousePosition,Tree)));
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

                if (OutputNode.userData is not GVNodeData outputNode)
                    throw new Exception("OutputNode.userData is not GVNodeData outputNode");

                if (InputNode.userData is not GVNodeData inputNode)
                    throw new Exception("InputNode.userData is not GVNodeData inputNode");

                if (inputNode == null)
                    throw new Exception("inputNode == null");

                EdgeData edgeData = new(edge.output.viewDataKey, edge.input.viewDataKey, edge.viewDataKey);
                Tree.AddEdge(edgeData);
            }
        }

        if (changes.elementsToRemove != null)
        {
            foreach (var element in changes.elementsToRemove)
            {
                if (element is Edge edge)
                {
                    Debug.Log("Edge was removed.");
                    EdgeData toRemoveEdgeData = Tree.Edges.FirstOrDefault(_edge => _edge.Id == edge.viewDataKey);
                    Tree.RemoveEdge(toRemoveEdgeData);
                }
                else if (element is Node node)
                {
                    Debug.Log("Node was removed.");
                    GVNodeData toRemoveNode = Tree.Nodes.FirstOrDefault(_node => _node.Id == node.viewDataKey);
                    Tree.Nodes.Remove(toRemoveNode);
                    AssetDatabase.RemoveObjectFromAsset(toRemoveNode);
                    //AssetDatabase.SaveAssets();
                }
            }
        }

        if (changes.movedElements != null)
        {
            foreach(var element in changes.movedElements)
            {
                if(element.userData is GVNodeData nodeData)
                {
                    nodeData.GraphPosition = element.GetPosition().position;
                }
            }
        }

        return changes;
    }

    void SaveGraph()
    {
        Debug.Log("Save");
        
        EditorUtility.SetDirty(Tree);
        AssetDatabase.SaveAssets();
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

