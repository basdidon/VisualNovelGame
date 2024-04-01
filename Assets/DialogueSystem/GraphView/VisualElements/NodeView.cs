using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;

namespace H8.GraphView.UiElements
{
    public class NodeView : Node
    {
        public DialogueGraphView GraphView { get; private set; }
        public SerializedObject SerializeObject { get; private set; }
        BaseNode baseNode;

        public void Initialize(BaseNode nodeData, DialogueGraphView graphView)
        {
            GraphView = graphView;
            SetPosition(new Rect(nodeData.GraphPosition, Vector2.zero));
            viewDataKey = nodeData.Id;
            userData = nodeData;
            name = nodeData.GetType().Name;
            title = name;

            Label titleLabel = (Label) titleContainer.ElementAt(0);
            titleLabel.bindingPath = "title";

            SerializeObject = new(nodeData);
            mainContainer.Bind(SerializeObject);

            baseNode = nodeData;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            Selection.activeObject = baseNode;
        }

        public virtual void OnDrawNodeView(BaseNode baseNode)
        {
            Debug.Log($"StartDrawNode : <color=yellow>{baseNode.GetType().Name}</color>");

            CreatePorts(baseNode);

            var fields = baseNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach(var field in fields)
            {
                CreateNodeField(field);
                CreateSelector(field);
                CreateListElement(field);
            }

            RefreshExpandedState();
        }

        void CreatePorts(BaseNode baseNode)
        {
            // create ports
            foreach (var portData in baseNode.Ports)
            {
                PropertyInfo property = baseNode.GetType().GetProperty(portData.FieldName);
                if (property == null)
                    continue;

                if (property.IsDefined(typeof(PortAttribute)))
                {
                    var portAttr = property.GetCustomAttribute<PortAttribute>();
                    VisualElement portContainer = portData.Direction == Direction.Input ? inputContainer : outputContainer;
                    Port port = portAttr.CreatePort(property, this, portData,SerializeObject);

                    portContainer.Add(port);
                }
            }
        }

        void CreateSelector(FieldInfo fieldInfo)
        {
            if (!fieldInfo.IsDefined(typeof(SelectorAttribute), inherit: true))
                return;

            EnumField enumField = new(StringHelper.ToCapitalCase(fieldInfo.Name)) { bindingPath = fieldInfo.Name };
            extensionContainer.Add(enumField);
        }
        
        void CreateNodeField(FieldInfo fieldInfo)
        {
            if (!fieldInfo.IsDefined(typeof(NodeFieldAttribute), inherit: true))
                return;

            var nodeFieldAttr = fieldInfo.GetCustomAttribute<NodeFieldAttribute>();
            extensionContainer.Add(nodeFieldAttr.CreatePropertyField(fieldInfo,SerializeObject));
        }

        void CreateListElement(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(ListElements<>))
            {
                var newList = new GraphListView(SerializeObject.FindProperty(fieldInfo.Name), this, fieldInfo.FieldType.GetGenericArguments()[0]);
                extensionContainer.Add(newList);
                RefreshExpandedState();
            }
        }

        public void RemovePort(Port port)
        {
            Debug.Log("removing port");
            if (port.connected)
            {
                Debug.Log("-- deleting edges");
                GraphView.DeleteElements(port.connections);
                port.DisconnectAll();
            }

            GraphView.RemoveElement(port);
        }
    }
}