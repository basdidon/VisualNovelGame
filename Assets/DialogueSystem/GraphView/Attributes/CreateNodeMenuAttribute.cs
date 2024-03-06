using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{

    [AttributeUsage(AttributeTargets.Class)]
    public class CreateNodeMenuAttribute : Attribute
    {
        public string menuName;

        static readonly Assembly[] assemblyToSearch = {
            Assembly.GetExecutingAssembly(),    // this assembly
            Assembly.Load("Assembly-CSharp"),   // default assembly in Assets/
        };

        [Obsolete]
        public static IEnumerable<BaseNode> GetAllBaseNode()
        {
            foreach (var assembly in assemblyToSearch)
            {
                var typesWithAttribute = assembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(BaseNode)) && GetCustomAttribute(type, typeof(CreateNodeMenuAttribute)) != null);

                foreach (var type in typesWithAttribute)
                {
                    var instance = ScriptableObject.CreateInstance(type) as BaseNode;
                    if (instance != null)
                        yield return instance;
                }
            }
        }

        public static void PopulateCreateContextualMenu(DialogueGraphView graphView)
        {
            foreach (var assembly in assemblyToSearch)
            {
                var typeMenuNamePair = assembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(BaseNode)) && GetCustomAttribute(type, typeof(CreateNodeMenuAttribute)) != null)
                    .Select(type => new { Type = type, (GetCustomAttribute(type, typeof(CreateNodeMenuAttribute)) as CreateNodeMenuAttribute).menuName });

                foreach (var pair in typeMenuNamePair)
                {
                    graphView.AddManipulator(new ContextualMenuManipulator(ev => {
                        ev.menu.AppendAction(pair.menuName, actionEvent => {
                            VisualElement contentViewContainer = graphView.ElementAt(1);
                            Vector3 screenMousePosition = actionEvent.eventInfo.localMousePosition;
                            Vector2 worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
                            worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
                            // create baseNode
                            BaseNode baseNode = NodeFactory.CreateNode(pair.Type, worldMousePosition, graphView.Tree);
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
}
