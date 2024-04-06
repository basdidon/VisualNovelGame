using UnityEngine;
using System;

namespace H8.GraphView.UiElements
{
    public static class NodeFactory
    {
        public static BaseNode CreateNode(Type type, Vector2 position, GraphTree dialogueTree)
        {
            BaseNode nodeData = (BaseNode) ScriptableObject.CreateInstance(type);
            if(nodeData != null)
            {
                nodeData.Initialize(position, dialogueTree);

                return nodeData;
            }

            throw new Exception();
        }
        public static BaseNode CreateNode<T>(Vector2 position, GraphTree dialogueTree) where T : BaseNode
        {
            var nodeData = ScriptableObject.CreateInstance<T>();
            nodeData.Initialize(position, dialogueTree);

            return nodeData;
        }

        public static NodeView GetNodeView<T>(T nodeData, DialogueGraphView graphView) where T : BaseNode
        {
            NodeView nodeView;

            if(CustomNodeViewAttribute.TryGetCustomGraphViewNodeType(nodeData.GetType(),out Type customNodeViewType))
            {
                var nodeType = customNodeViewType;
                nodeView = Activator.CreateInstance(nodeType) as NodeView;
                Debug.Log($"CreateNodeView {nodeType.Name}");
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