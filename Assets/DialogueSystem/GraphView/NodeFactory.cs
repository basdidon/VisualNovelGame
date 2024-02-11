using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CustomGraphViewNode(typeof(NodeFactory))]
    public static class NodeFactory
    {
        public static BaseNode CreateNode<T>(Vector2 position, DialogueTree dialogueTree) where T : BaseNode
        {
            var nodeData = ScriptableObject.CreateInstance<T>();
            nodeData.Initialize(position, dialogueTree);

            return nodeData;
        }

        public static GraphViewNode GetNodeView<T>(T nodeData, DialogueGraphView graphView) where T : BaseNode
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var typesWithAttribute = allTypes
                .Where(t => t.IsSubclassOf(typeof(GraphViewNode)) && Attribute.IsDefined(t, typeof(CustomGraphViewNodeAttribute)));

            var nodeTypeAttribute = typesWithAttribute
                .Select(t => new { Type = t, Attribute = (CustomGraphViewNodeAttribute)Attribute.GetCustomAttribute(t, typeof(CustomGraphViewNodeAttribute)) })
                .SingleOrDefault(item => item.Attribute != null && item.Attribute.Type == nodeData.GetType());

            GraphViewNode instance;

            if (nodeTypeAttribute != null)
            {
                var nodeType = nodeTypeAttribute.Type;
                instance = Activator.CreateInstance(nodeType) as GraphViewNode;
                Debug.Log($"GetNodeView {nodeType.Name}");
            }
            else
            {
                instance = Activator.CreateInstance<DefaultGraphViewNode>();
            }

            instance.Initialize(nodeData,graphView);
            instance.OnDrawNodeView(nodeData);
            return instance;

        }
    }
}