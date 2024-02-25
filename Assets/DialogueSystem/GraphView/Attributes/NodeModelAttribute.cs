using BasDidon.Dialogue.VisualGraphView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

[AttributeUsage(AttributeTargets.Class)]
public class NodeModelAttribute : Attribute
{
    public string menuName;

    static readonly Assembly[] assemblyToSearch = {
            Assembly.GetExecutingAssembly(),    // this assembly
            Assembly.Load("Assembly-CSharp"),   // default assembly in Assets/
        };

    public static void PopulateCreateContextualMenu(DialogueGraphView graphView)
    {
        foreach (var assembly in assemblyToSearch)
        {
            var typeMenuNamePair = assembly.GetTypes()
                .Where(type => GetCustomAttribute(type, typeof(NodeModelAttribute)) != null)
                .Select(type => new { Type = type, (GetCustomAttribute(type, typeof(NodeModelAttribute)) as NodeModelAttribute).menuName });


            foreach (var pair in typeMenuNamePair)
            {
                var modelMethods = pair.Type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                foreach(var modelMethod in modelMethods)
                {
                    string createMenuName = $"{pair.Type.Name}/{modelMethod.Name}";

                    graphView.AddManipulator(new ContextualMenuManipulator(ev => {
                        ev.menu.AppendAction(createMenuName, actionEvent => {
                            /*
                            // create baseNode
                            BaseNode baseNode = NodeFactory.CreateNode(pair.Type, actionEvent.eventInfo.localMousePosition, graphView.Tree);
                            // get nodeView by baseNode
                            NodeView nodeView = NodeFactory.GetNodeView(baseNode, graphView);
                            // add nodeView to graph
                            graphView.AddElement(nodeView);
                            */
                        });
                    }));
                }
                

            }
        }
    }
}
