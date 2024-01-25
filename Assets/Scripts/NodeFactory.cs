using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphview.NodeData;
using System;
using System.Linq;
using System.Reflection;

namespace Graphview.NodeView
{
    [CustomGraphViewNode(typeof(NodeFactory))]
    public static class NodeFactory
    {
        public static GVNodeData CreateNode<T>(Vector2 position, DialogueTree dialogueTree) where T : GVNodeData
        {
            var nodeData = ScriptableObject.CreateInstance<T>();
            nodeData.Initialize(position, dialogueTree);

            return nodeData;
        }

        public static GraphViewNode LoadNode(GVNodeData nodeData)
        {
            Debug.Log($"load node {nodeData.name}");
            return nodeData.GetNodeView();
        }

        public static GraphViewNode GetNodeView<T>(T nodeData) where T : GVNodeData
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
            }
            else
            {
                instance = Activator.CreateInstance<GraphViewNode>();
            }

            instance.Initialize(nodeData);
            instance.OnDrawNodeView(nodeData);
            return instance;

        }
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = false)]
    public class CustomGraphViewNodeAttribute : Attribute
    {
        public Type Type;
        public CustomGraphViewNodeAttribute(Type type)
        {
            Type = type;
        }
    }
}