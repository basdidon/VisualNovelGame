using UnityEngine;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    public static class NodeFactory
    {
        public static BaseNode CreateNode(Type type, Vector2 position, DialogueTree dialogueTree)
        {
            BaseNode nodeData = (BaseNode) ScriptableObject.CreateInstance(type);
            if(nodeData != null)
            {
                nodeData.Initialize(position, dialogueTree);

                return nodeData;
            }

            throw new Exception();
        }
        public static BaseNode CreateNode<T>(Vector2 position, DialogueTree dialogueTree) where T : BaseNode
        {
            var nodeData = ScriptableObject.CreateInstance<T>();
            nodeData.Initialize(position, dialogueTree);

            return nodeData;
        }

        public static NodeView GetNodeView<T>(T nodeData, DialogueGraphView graphView) where T : BaseNode
        {
            var customGraphViewNodeType = CustomGraphViewNodeAttribute.GetGraphViewNodeType(nodeData.GetType());

            NodeView nodeView;

            if (customGraphViewNodeType != null)
            {
                var nodeType = customGraphViewNodeType;
                nodeView = Activator.CreateInstance(nodeType) as NodeView;
                Debug.Log($"GetNodeView {nodeType.Name}");
            }
            else
            {
                nodeView = Activator.CreateInstance<NodeView>();
            }

            nodeView.Initialize(nodeData,graphView);
            nodeView.OnDrawNodeView(nodeData);
            return nodeView;
        }
    }
}