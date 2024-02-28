
using UnityEditor.Experimental.GraphView;


namespace BasDidon.Dialogue.VisualGraphView
{
    public abstract class CustomPortType
    {
        public abstract Port CreatePort(string portGuid, Direction portDirection, NodeView nodeView, string propertyName);
    }
}