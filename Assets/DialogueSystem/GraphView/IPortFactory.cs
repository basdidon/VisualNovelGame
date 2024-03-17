using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    public interface IPortFactory
    {
        Port CreatePort(string portGuid, Direction direction, NodeView nodeView, string portName);
        Port CreatePortWithField(SerializedProperty serializedProperty, string portGuid, Direction direction, NodeView nodeView, string propertyName);
        //
        Port CreateUnbindPort(Direction direction, NodeView nodeView, string propertyName);
        Port CreateUnbindPortWithField(Direction direction, NodeView nodeView, string propertyName);
        void BindPort(VisualElement e, string propertyName, string portGuid, PortAttribute portAttr, SerializedProperty serializedProperty = null);
    }
}