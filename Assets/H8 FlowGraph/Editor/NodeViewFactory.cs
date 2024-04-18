using System;
using UnityEngine;

namespace H8.FlowGraph.UiElements
{
    public class NodeViewFactory
    {
        public static NodeView GetNodeView<T>(T nodeData, DialogueGraphView graphView) where T : BaseNode
        {
            NodeView nodeView;

            if (CustomNodeViewAttribute.TryGetCustomGraphViewNodeType(nodeData.GetType(), out Type customNodeViewType))
            {
                var nodeType = customNodeViewType;
                nodeView = Activator.CreateInstance(nodeType) as NodeView;
                Debug.Log($"CreateNodeView {nodeType.Name}");
            }
            else
            {
                nodeView = Activator.CreateInstance<NodeView>();
            }

            nodeView.Initialize(nodeData, graphView);
            nodeView.OnDrawNodeView(nodeData);
            return nodeView;
        }
    }
}
