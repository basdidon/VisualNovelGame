using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
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

        // init node
        StartNode = CreateNode<StartNode>(new Vector2(0, 250));
        AddElement(StartNode);

        m_tree = AssetDatabase.LoadAssetAtPath<DialogueTree>(assetPath);
        if (m_tree == null)
        {
            m_tree = ScriptableObject.CreateInstance<DialogueTree>();
            AssetDatabase.CreateAsset(m_tree, assetPath);
        }
        else
        {
            
        }

        graphViewChanged += OnGraphViewChange;
    }

    public void PopulateView()
    {
       // m_tree.Dialogues.ForEach()

    }

    BaseNode LoadNode()
    {
        throw new System.NotImplementedException();
    }

    BaseNode CreateNode<T>(Vector2 position) where T:BaseNode
    {        
        BaseNode node = Activator.CreateInstance<T>();
        node.Initialize(position);
        node.Draw();

        if (node.GetType().IsSubclassOf(typeof(DialogueBaseNode)))
        {
            (node as DialogueBaseNode).DialoguesData = CreateNodeAsset();
        }

        return node;
    }

    DialogueNodeData CreateNodeAsset()
    {
        var nodeData = ScriptableObject.CreateInstance<DialogueNodeData>();
        AssetDatabase.AddObjectToAsset(nodeData, m_tree);
        m_tree.Dialogues.Add(nodeData);
        AssetDatabase.SaveAssets();
        return nodeData;
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
            actionEvent => AddElement(CreateNode<DialogueNode>(actionEvent.eventInfo.localMousePosition)));
        evt.menu.AppendAction($"Create ConditionNode",
            actionEvent => AddElement(CreateNode<ConditionNode>(actionEvent.eventInfo.localMousePosition)));
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
                Debug.Log($"{edge.output.node.title} -> {edge.input.node.title}");
                if (edge.output.node.userData.GetType() == typeof(StartNode))
                {
                    m_tree.StartDialogue = (edge.input.node.userData as DialogueBaseNode).DialoguesData;
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
