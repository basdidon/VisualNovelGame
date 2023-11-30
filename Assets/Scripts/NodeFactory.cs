using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphview.NodeData;

public static class NodeFactory
{
    public static GraphViewNode CreateNode<T>(Vector2 position, DialogueTree dialogueTree) where T : GVNodeData
    {
        var nodeData = ScriptableObject.CreateInstance<T>();
        nodeData.Initialize(position, dialogueTree);

        return nodeData.CreateNode() as GraphViewNode;
    }

    public static GraphViewNode LoadNode(GVNodeData nodeData)
    {
        return nodeData.CreateNode() as GraphViewNode;
    }
}
