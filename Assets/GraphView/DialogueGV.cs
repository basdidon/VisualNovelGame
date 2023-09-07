using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Collections.Generic;

public class DialogueGV : GraphView
{
    public DialogueGV()
    {
        AddManipulator();
        AddBackground();
        AddStyle();
    }

    BaseNode CreateNode(BaseNode.NodeTypes nodeType,Vector2 position)
    {
        Type _type = Type.GetType($"{nodeType}");
        BaseNode node = (BaseNode) Activator.CreateInstance(_type);
        node.Initialize(position);
        node.Draw();
        return node;
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

        this.AddManipulator(CreateNodeContextualMenu("Add Dialogue Node", BaseNode.NodeTypes.DialogueNode));
        this.AddManipulator(CreateNodeContextualMenu("Add Condition Node", BaseNode.NodeTypes.ConditionNode));
    }

    IManipulator CreateNodeContextualMenu(string actionTitle, BaseNode.NodeTypes nodeType)
    {
        return new ContextualMenuManipulator ( menuEvent => 
            {
                menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(nodeType, actionEvent.eventInfo.localMousePosition)));
            }
        );
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
