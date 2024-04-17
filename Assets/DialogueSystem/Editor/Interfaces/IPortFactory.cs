using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace H8.GraphView.UiElements
{
    public interface IPortFactory
    {
        Port CreatePort(string portGuid, Direction direction, NodeView nodeView, string portName);
        Port CreatePortWithField(SerializedProperty serializedProperty, string portGuid, Direction direction, NodeView nodeView, string propertyName);
        //
        Port CreateUnbindPort(Direction direction, NodeView nodeView, string propertyName,bool HasBackingField = false);
        void BindPort(VisualElement e, string propertyName, string portGuid, PortAttribute portAttr, SerializedProperty serializedProperty = null);
    }
}