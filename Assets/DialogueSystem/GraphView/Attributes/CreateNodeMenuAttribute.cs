using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace H8.GraphView
{
    using UiElements;

    [AttributeUsage(AttributeTargets.Class)]
    public class CreateNodeMenuAttribute : Attribute
    {
        public string MenuName { get; }

        public CreateNodeMenuAttribute(string menuName)
        {
            MenuName = menuName;
        }

        public static void PopulateCreateContextualMenu(DialogueGraphView graphView)
        {
            var baseNodesType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BaseNode)) && type.IsDefined(typeof(CreateNodeMenuAttribute)));
           
            foreach(var type in baseNodesType)
            {
                var attr = type.GetCustomAttribute<CreateNodeMenuAttribute>();

                graphView.AddManipulator(new ContextualMenuManipulator(ev =>
                {
                    ev.menu.AppendAction(attr.MenuName, actionEvent =>
                    {
                        VisualElement contentViewContainer = graphView.ElementAt(1);
                        Vector3 screenMousePosition = actionEvent.eventInfo.localMousePosition;
                        Vector2 worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
                        worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
                        // create baseNode
                        BaseNode baseNode = NodeFactory.CreateNode(type, worldMousePosition, graphView.Tree);
                        // get nodeView by baseNode
                        NodeView nodeView = NodeFactory.GetNodeView(baseNode, graphView);                       

                        // add nodeView to graph
                        graphView.AddElement(nodeView);
                    });
                }));
                
            }
        }
    }
}
