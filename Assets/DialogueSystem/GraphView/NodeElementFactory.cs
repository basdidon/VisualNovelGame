using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    public interface IPortFactory
    {
        Port CreatePort(string portGuid, Direction direction, NodeView nodeView, string portName);
        Port CreatePortWithField(SerializedProperty serializedProperty, string portGuid, Direction direction, NodeView nodeView, string propertyName);
    }

    public static class NodeElementFactory
    {
        // find match Factory class for specific type
        static IPortFactory GetPortFactory(Type type)
        {
            if(type == typeof(ExecutionFlow))
            {
                return new ExecutionFlowPortFactory();
            }
            else if(PrimativeTypePortFactory.TryCreateFactory(type,out PrimativeTypePortFactory factory))
            {
                return factory;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static void DrawPort(Type type, PortData portData, NodeView nodeView, string portName)
        {
            VisualElement portContainer = portData.Direction == Direction.Input ? nodeView.inputContainer : nodeView.outputContainer;
            portContainer.Add(CreatePort(type,portData,nodeView,portName));
        }

        public static Port CreatePort(Type type, PortData portData, NodeView nodeView, string portName)
            => CreatePort(type, portData.PortGuid, portData.Direction, nodeView, portName);
        public static Port CreatePort(Type type, string portGuid, Direction direction, NodeView nodeView, string portName)
        {
            IPortFactory factory = GetPortFactory(type);
            return factory.CreatePort(portGuid, direction, nodeView, portName);
        }
        
        ///
        public static void DrawPortWithField(SerializedProperty serializedProperty, Type type, PortData portData, NodeView nodeView, string propertyName)
        {
            VisualElement portContainer = portData.Direction == Direction.Input ? nodeView.inputContainer : nodeView.outputContainer;
            portContainer.Add(CreatePortWithField(serializedProperty, type, portData, nodeView, propertyName));
        }

        // CreatePortWithField
        public static Port CreatePortWithField(SerializedProperty serializedProperty, Type type, PortData portData, NodeView nodeView, string propertyName)
            => CreatePortWithField(serializedProperty, type, portData.PortGuid, portData.Direction, nodeView, propertyName);
        public static Port CreatePortWithField(SerializedProperty serializedProperty, Type type, string portGuid, Direction direction, NodeView nodeView, string propertyName)
        {
            IPortFactory factory = GetPortFactory(type);
            return factory.CreatePortWithField(serializedProperty, portGuid, direction, nodeView, propertyName);
        }

    }
}